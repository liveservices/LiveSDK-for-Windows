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