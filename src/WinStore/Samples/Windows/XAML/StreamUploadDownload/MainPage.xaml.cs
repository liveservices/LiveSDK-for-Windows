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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StreamUploadDownload
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly string[] scopes = new string[]{"wl.signin", "wl.basic", "wl.skydrive_update"};

        private LiveAuthClient authClient;
        private LiveConnectClient liveClient;
        private CancellationTokenSource cts;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitializePage();
            CheckPendingBackgroundOperations();
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

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            this.UploadFile();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            this.DownloadFile();
        }

        private async void UploadFile()
        {
            if (this.liveClient == null)
            {
                this.ShowMessage("Please sign in first.");
                return;
            }

            try
            {
                string folderPath = this.tbUrl.Text;
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add("*");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (IInputStream inStream = await file.OpenReadAsync().AsTask())
                    {
                        this.progressBar.Value = 0;
                        this.ShowProgress();
                        var progressHandler = new Progress<LiveOperationProgress>(
                            (progress) => { this.progressBar.Value = progress.ProgressPercentage; });

                        this.cts = new CancellationTokenSource();
                        LiveUploadOperation operation = 
                            await this.liveClient.CreateBackgroundUploadAsync(
                                folderPath, 
                                file.Name, 
                                inStream, 
                                OverwriteOption.Rename);
                        LiveOperationResult result = await operation.StartAsync(this.cts.Token, progressHandler);

                        if (result != null)
                        {
                            this.progressBar.Value = 0;
                            dynamic fileData = result.Result;
                            string downloadUrl = fileData.id + "/picture";
                            this.tbUrl.Text = downloadUrl;

                            this.ShowMessage("Upload completed.");
                        }
                        else
                        {
                            this.ShowMessage("Upload failed.");
                        }
                    }
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

        private async void DownloadFile()
        {
            if (this.liveClient == null)
            {
                this.ShowMessage("Please sign in first.");
                return;
            }

            try
            {
                string filePath = this.tbUrl.Text;
                if (string.IsNullOrEmpty(filePath))
                {
                    this.ShowMessage("Please specify a file id or a url.");
                }
                else
                {
                    this.progressBar.Value = 0;
                    this.ShowProgress();
                    var progressHandler = new Progress<LiveOperationProgress>(
                        (progress) => { this.progressBar.Value = progress.ProgressPercentage; });

                    this.cts = new CancellationTokenSource();
                    LiveDownloadOperation op =
                        await this.liveClient.CreateBackgroundDownloadAsync(filePath);
                    LiveDownloadOperationResult downloadResult = await op.StartAsync(this.cts.Token, progressHandler);
                    if (downloadResult.Stream != null)
                    {
                        using (IRandomAccessStream ras = await downloadResult.GetRandomAccessStreamAsync())
                        {
                            var imgSource = new BitmapImage();
                            imgSource.SetSource(ras);
                            this.imgPreview.Source = imgSource;

                            this.ShowImage();
                        }
                    }
                    else
                    {
                        this.ShowMessage("Download failed.");
                    }
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
            this.imgPreview.Visibility = Visibility.Collapsed;
            this.tbMessage.Visibility = Visibility.Collapsed;
            this.progressBar.Value = 0;
            this.progressBar.Visibility = Visibility.Visible;
            this.btnCancel.IsEnabled = true;
        }

        private void ShowMessage(string message)
        {
            this.imgPreview.Visibility = Visibility.Collapsed;
            this.tbMessage.Text = message;
            this.tbMessage.Visibility = Visibility.Visible;
            this.progressBar.Visibility = Visibility.Collapsed;
            this.btnCancel.IsEnabled = false;
        }

        private void ShowImage()
        {
            this.tbMessage.Visibility = Visibility.Collapsed;
            this.imgPreview.Visibility = Visibility.Visible;
            this.progressBar.Visibility = Visibility.Collapsed;
            this.btnCancel.IsEnabled = false;
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
