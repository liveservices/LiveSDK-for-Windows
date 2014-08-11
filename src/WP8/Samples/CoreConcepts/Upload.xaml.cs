// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;

namespace CoreConcepts
{

    public partial class Upload : PhoneApplicationPage, IProgress<LiveOperationProgress>
    {
        private LiveConnectClient liveClient;
        private readonly DispatcherTimer timer;

        public Upload()
        {
            InitializeComponent();
            
            this.timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 10),
            };
            this.timer.Tick += OnTimer_Tick;
            this.timer.Start();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Loaded -= this.OnLoaded;

            if (MainPage.Session == null)
            {
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                this.liveClient = new LiveConnectClient(MainPage.Session);
                this.SelectFile.IsEnabled = true;
                this.SelectPicture.IsEnabled = true;
            }
        }

        public void Report(LiveOperationProgress value)
        {
            this.ProgressBar.Value = value.ProgressPercentage;
        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.MemoryMonitor.Text = String.Format(
                    "cur: {0} av: {1} lim: {2}",
                    DeviceStatus.ApplicationCurrentMemoryUsage,
                    DeviceStatus.ApplicationMemoryUsageLimit - DeviceStatus.ApplicationCurrentMemoryUsage,
                    DeviceStatus.ApplicationMemoryUsageLimit);
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex.ToString());
            }
        }

        private void SelectPicture_Click(object sender, RoutedEventArgs e)
        {
            var picker = new PhotoChooserTask();
            picker.Completed += OnPhotoChooserTaskCompleted;
            picker.Show();
        }

        private async void OnPhotoChooserTaskCompleted(object sender, PhotoResult e)
        {
            if (e.Error != null)
            {
                this.ShowMessage(e.Error.Message);
            }
            else if (e.OriginalFileName == null)
            {
                this.ShowMessage("No File Choosen.");
            }
            else
            {
                string[] filePathSegments =
                    e.OriginalFileName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string fileName = filePathSegments[filePathSegments.Length - 1];

                string uploadLocation = this.TextBoxUrl.Text;
                this.ShowProgress();

                try
                {
                    LiveOperationResult operationResult = await this.liveClient.UploadAsync(
                        uploadLocation, 
                        fileName, 
                        e.ChosenPhoto, 
                        OverwriteOption.Overwrite,
                        new CancellationToken(false),
                        this);

                    e.ChosenPhoto.Dispose();

                    dynamic result = operationResult.Result;
                    if (result.source != null)
                    {
                        this.ShowMessage("file uploaded \n" + result.source);
                    }
                }
                catch (LiveConnectException exception)
                {
                    this.ShowMessage(exception.Message);
                }
            }
        }

        private void ShowMessage(string message)
        {
            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.FileListPanel.Visibility = Visibility.Collapsed;

            this.Message.Text = message;
            this.Message.Visibility = Visibility.Visible;
        }

        private void ShowProgress()
        {
            this.Message.Visibility = Visibility.Collapsed;
            this.FileListPanel.Visibility = Visibility.Collapsed;

            this.ProgressBar.Value = 0;
            this.ProgressBar.Visibility = Visibility.Visible;
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] files = myStore.GetFileNames();

                this.FileListPanel.Children.Clear();
                foreach (string fileName in files)
                {
                    var fileButton = new Button()
                    {
                        Content = fileName,
                        Height = 70,
                        Width = 400,
                    };

                    fileButton.Click += OnFileSelected;

                    this.FileListPanel.Children.Add(fileButton);
                }

                this.ShowFileList();
            }
        }

        private async void OnFileSelected(object sender, RoutedEventArgs e)
        {
            var fileButton = sender as Button;
            var fileName = fileButton.Content as string;

            using (IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var isoFileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, myStore);
                string uploadLocation = this.TextBoxUrl.Text;
                this.ShowProgress();

                try
                {
                    LiveOperationResult operationResult = await this.liveClient.UploadAsync(
                        uploadLocation,
                        fileName,
                        isoFileStream,
                        OverwriteOption.Rename,
                        new CancellationToken(false),
                        this);
                    isoFileStream.Dispose();

                    dynamic result = operationResult.Result;
                    if (result.source != null)
                    {
                        this.ShowMessage("file uploaded \n" + result.source);
                    }
                }
                catch(LiveConnectException exception)
                {
                    this.ShowMessage(exception.Message);
                }
            }
        }

        private void ShowFileList()
        {
            this.Message.Visibility = Visibility.Collapsed;
            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.imgPreview.Visibility = Visibility.Collapsed;

            this.FileListPanel.Visibility = Visibility.Visible;
        }
    }
}