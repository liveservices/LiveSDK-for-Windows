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

//-------------------------------------------------------------------------------------------------
// <copyright file="MainPage.cs" company="Microsoft">
//   Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

using Microsoft.Live;
using Microsoft.Live.Controls;

namespace StreamUploadDownload
{
    using System.Threading;

    public partial class MainPage : PhoneApplicationPage
    {
        private LiveConnectClient liveClient;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            this.liveClient = (e.Status == LiveConnectSessionStatus.Connected) ? new LiveConnectClient(e.Session) : null;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            this.tbMessage.Visibility = Visibility.Collapsed;
            if (this.liveClient == null)
            {
                this.ShowMessage("Please sign in first.");
                return;
            }

            var picker = new PhotoChooserTask();
            picker.Completed += OnPhotoChooserTaskCompleted;
            picker.Show();
        }

        private async void OnPhotoChooserTaskCompleted(object sender, PhotoResult e)
        {
            if (e.Error != null)
            {
                this.ShowMessage(e.Error.ToString());
            }
            else if (e.OriginalFileName == null)
            {
                this.ShowMessage("No Photo Choosen.");
            }
            else
            {
                string[] filePathSegments = e.OriginalFileName.Split('\\');
                string fileName = filePathSegments[filePathSegments.Length - 1];

                try
                {
                    var progressHandler = new Progress<LiveOperationProgress>((LiveOperationProgress progress) =>
                    {
                        this.progressBar.Value = progress.ProgressPercentage;
                    });
                    this.ShowProgress();

                    LiveOperationResult operationResult = await this.liveClient.UploadAsync(
                        this.tbUrl.Text, 
                        fileName, 
                        e.ChosenPhoto, 
                        OverwriteOption.Rename, 
                        new CancellationToken(false), 
                        progressHandler);

                    e.ChosenPhoto.Dispose();

                    this.progressBar.Value = 0;
                    dynamic result = operationResult.Result;
                    string fileLocation = result.source;
                    LiveDownloadOperationResult downloadOperationResult =
                        await this.liveClient.DownloadAsync(fileLocation, new CancellationToken(false), progressHandler);
                    using (Stream downloadStream = downloadOperationResult.Stream)
                    {
                        if (downloadStream != null)
                        {
                            var imgSource = new BitmapImage();
                            imgSource.SetSource(downloadStream);
                            this.imgPreview.Source = imgSource;

                            this.ShowImage();
                        }
                    }
                }
                catch (Exception exp)
                {
                    this.ShowMessage(exp.ToString());
                }
            }
        }

        private void ShowMessage(string message)
        {
            this.progressBar.Visibility = Visibility.Collapsed;
            this.imgPreview.Visibility = Visibility.Collapsed;

            this.tbMessage.Text = message;
            this.tbMessage.Visibility = Visibility.Visible;
        }

        private void ShowImage()
        {
            this.progressBar.Visibility = Visibility.Collapsed;
            this.tbMessage.Visibility = Visibility.Collapsed;

            this.imgPreview.Visibility = Visibility.Visible;
        }

        private void ShowProgress()
        {
            this.imgPreview.Visibility = Visibility.Collapsed;
            this.tbMessage.Visibility = Visibility.Collapsed;

            this.progressBar.Visibility = Visibility.Visible;
        }
    }
}