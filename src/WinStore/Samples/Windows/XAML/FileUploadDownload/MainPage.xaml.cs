// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FileUploadDownload
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly string[] scopes = new string[]{ "wl.signin", "wl.basic", "wl.skydrive_update" };

        private LiveAuthClient authClient;
        private LiveConnectClient liveClient;
        private CancellationTokenSource cts;
        private string fileName;

        public MainPage()
        {
            this.InitializeComponent();

            CheckPendingBackgroundOperations();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitializePage();
        }

        private async void InitializePage()
        {
            try
            {
                this.authClient = new LiveAuthClient();
                LiveLoginResult loginResult = await this.authClient.InitializeAsync(scopes);
                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    if (this.authClient.CanLogout)
                    {
                        this.btnLogin.Content = "Sign Out";
                    }
                    else
                    {
                        this.btnLogin.Visibility = Visibility.Collapsed;
                    }

                    this.liveClient = new LiveConnectClient(loginResult.Session);
                }
            }
            catch (LiveAuthException)
            {
                // TODO: Display the exception
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.btnLogin.Content.ToString() == "Sign In")
                {
                    LiveLoginResult loginResult = await this.authClient.LoginAsync(scopes);
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        if (this.authClient.CanLogout)
                        {
                            this.btnLogin.Content = "Sign Out";
                        }
                        else
                        {
                            this.btnLogin.Visibility = Visibility.Collapsed;
                        }

                        this.liveClient = new LiveConnectClient(loginResult.Session);
                    }
                }
                else
                {
                    this.authClient.Logout();
                    this.btnLogin.Content = "Sign In";
                }
            }
            catch (LiveAuthException)
            {
                // TODO: Display the exception
            }
        }

        private async void btnSelectUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.liveClient == null)
            {
                this.ShowMessage("Please sign in first.");
                return;
            }

            if (string.IsNullOrEmpty(this.tbuploadUrl.Text))
            {
                this.ShowMessage("Please specify the upload folder path.");
                return;
            }

            try
            {
                string folderPath = this.tbuploadUrl.Text;
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                picker.FileTypeFilter.Add("*");
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    this.fileName = file.Name;
                    this.progressBar.Value = 0;
                    var progressHandler = new Progress<LiveOperationProgress>(
                        (progress) => { this.progressBar.Value = progress.ProgressPercentage; });

                    this.ShowProgress();
                    this.cts = new CancellationTokenSource();

                    LiveUploadOperation operation = await this.liveClient.CreateBackgroundUploadAsync(
                        folderPath, 
                        file.Name, 
                        file, 
                        OverwriteOption.Rename);
                    LiveOperationResult result = await operation.StartAsync(
                        this.cts.Token, 
                        progressHandler);

                    dynamic fileData = result.Result;
                    string downloadUrl = fileData.id + "/content";
                    this.tbdownloadUrl.Text = downloadUrl;

                    this.ShowMessage("Upload completed");
                }
            }
            catch (TaskCanceledException)
            {
                this.ShowMessage("User has cancelled the operation.");
            }
            catch (Exception exp)
            {
                this.ShowMessage(exp.ToString());
            }
        }

        private async void btnSelectDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.liveClient == null)
            {
                this.ShowMessage("Please sign in first.");
                return;
            }

            if (string.IsNullOrEmpty(this.tbdownloadUrl.Text))
            {
                this.ShowMessage("Please specify the link to the file to be downloaded.");
                return;
            }

            try
            {
                string fileLink = this.tbdownloadUrl.Text;

                var roamingSettings = ApplicationData.Current.RoamingSettings;
                roamingSettings.Values["FileName"] = this.fileName;

                var appSettingContainer = roamingSettings.CreateContainer(
                    "FileUploadDownload Settings", 
                    ApplicationDataCreateDisposition.Always);
                appSettingContainer.Values[this.fileName] = true;

                var roamingFolder = ApplicationData.Current.RoamingFolder;
                var storageDir = await roamingFolder.CreateFolderAsync(
                    "FileUploadDownload sample", 
                    CreationCollisionOption.OpenIfExists);

                var storageFile =
                    await storageDir.CreateFileAsync(this.fileName, CreationCollisionOption.ReplaceExisting);

                if (storageFile != null)
                {
                    this.progressBar.Value = 0;
                    var progressHandler = new Progress<LiveOperationProgress>(
                        (progress) => { this.progressBar.Value = progress.ProgressPercentage; });

                    this.ShowProgress();
                    this.cts = new CancellationTokenSource();

                    LiveDownloadOperation operation = await this.liveClient.CreateBackgroundDownloadAsync(
                        fileLink, 
                        storageFile);
                    LiveDownloadOperationResult result = await operation.StartAsync(this.cts.Token, progressHandler);

                    this.ShowMessage("Download completed.");
                }
            }
            catch (TaskCanceledException)
            {
                this.ShowMessage("User has cancelled the operation.");
            }
            catch (Exception exp)
            {
                this.ShowMessage(exp.ToString());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.cts != null)
            {
                this.cts.Cancel();
            }
        }

        private void ShowProgress()
        {
            this.btnCancel.IsEnabled = true;
            this.tbMessage.Visibility = Visibility.Collapsed;
            this.progressBar.Value = 0;
            this.progressBar.Visibility = Visibility.Visible;
        }

        private void ShowMessage(string message)
        {
            this.btnCancel.IsEnabled = false;
            this.tbMessage.Text = message;
            this.tbMessage.Visibility = Visibility.Visible;
            this.progressBar.Visibility = Visibility.Collapsed;
        }

        private async void CheckPendingBackgroundOperations()
        {
            var downloadOperations = await Microsoft.Live.LiveConnectClient.GetCurrentBackgroundDownloadsAsync();
            foreach (var operation in downloadOperations)
            {
                try
                {
                    var result = await operation.AttachAsync();

                    // Process download completed results.
                }
                catch (Exception ex)
                {
                    // Handle download error
                }
            }

            var uploadOperations = await Microsoft.Live.LiveConnectClient.GetCurrentBackgroundUploadsAsync();
            foreach (var operation in uploadOperations)
            {
                try
                {
                    var result = await operation.AttachAsync();
                    // Process upload completed results.
                }
                catch (Exception ex)
                {
                    // Handle upload error
                }
            }
        }
    }
}
