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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LiveSDKPhoneRTSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UploadDownload : Page
    {
        public UploadDownload()
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
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            string message = null;
            try
            {
                // Ensure that the user has consented the wl.skydrive and wl.skydrive_update scopes
                var authClient = new LiveAuthClient();
                var authResult = await authClient.LoginAsync(new string[] { "wl.skydrive", "wl.skydrive_update" });
                if (authResult.Session == null)
                {
                    throw new InvalidOperationException("You need to sign in and give consent to the app.");
                }

                var liveConnectClient = new LiveConnectClient(authResult.Session);
                // Validate parameters
                var imageSourceUrl = imgSourceUrlTextBox.Text;
                if (string.IsNullOrWhiteSpace(imageSourceUrl))
                {
                    throw new InvalidOperationException("Image Url is empty.");
                }

                var uploadPath = uploadPathTextBox.Text;
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    throw new InvalidOperationException("Upload location is empty.");
                }

                // Download the image from the Internet                
                var networkFileDownloader = new BackgroundDownloader();
                DownloadOperation networkFileDownloadOperation = networkFileDownloader.CreateDownload(new Uri(imageSourceUrl), null);
                await networkFileDownloadOperation.StartAsync();
                IInputStream uploadInputStream = networkFileDownloadOperation.GetResultStreamAt(0);


                // Upload to OneDrive
                LiveUploadOperation uploadOperation = await liveConnectClient.CreateBackgroundUploadAsync(uploadPath, "ImageFromInternet.jpg", uploadInputStream, OverwriteOption.Rename);
                LiveOperationResult uploadResult = await uploadOperation.StartAsync();
                dynamic uploadedResource = uploadResult.Result;
                string uploadedResourceId = uploadedResource.id;
                string uploadedResourceName = uploadedResource.name;
                string uploadedResourceLink = uploadedResource.source;
                uploadedResourceTextBox.Text = string.Format("{0}\r\n{1}\r\n{2}", uploadedResourceId, uploadedResourceName, uploadedResourceLink);

                // Download from the OneDrive
                LiveDownloadOperation downloadOperation = await liveConnectClient.CreateBackgroundDownloadAsync(uploadedResourceId + "/content");
                LiveDownloadOperationResult downloadResult = await downloadOperation.StartAsync();
                if (downloadResult != null && downloadResult.Stream != null)
                {
                    using (IRandomAccessStream ras = await downloadResult.GetRandomAccessStreamAsync())
                    {
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.SetSource(ras);
                        skydriveImage.Source = imgSource;
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Operation failed: " + ex.Message;
            }

            if (message != null)
            {
                var dialog = new Windows.UI.Popups.MessageDialog(message);
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            uploadedResourceTextBox.Text = string.Empty;
            skydriveImage.Source = null;
        }
    }
}
