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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainPage : Page
    {
        private readonly string[] _defaultAuthScopes = new string[] { "wl.signin", "wl.skydrive" };

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
            CheckUserStatus();
        }

        private async void CheckUserStatus()
        {
            await AuthenticateUser(true);
        }

        private void UpdateUI(bool connected, string firstName)
        {
            greatingTextBlock.Text = connected ? "Helo " + firstName : string.Empty;
            statusTextBlock.Text = connected ? "You are connected!" : "You are not connected. \r\nTap the button below to start.";
            connectButton.Visibility = connected ? Visibility.Collapsed : Visibility.Visible;            
        }

        private async Task LoadProfileImage(LiveConnectClient connectClient)
        {
            try
            {
                LiveDownloadOperation downloadOperation = await connectClient.CreateBackgroundDownloadAsync("me/picture");
                LiveDownloadOperationResult result = await downloadOperation.StartAsync();
                if (result != null && result.Stream != null)
                {
                    using (IRandomAccessStream ras = await result.GetRandomAccessStreamAsync())
                    {
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.SetSource(ras);
                        profileImage.Source = imgSource;
                    }
                }
            }
            catch (LiveConnectException)
            {
                // Handle error cases.
            }
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            await AuthenticateUser(false);
        }

        private async Task AuthenticateUser(bool silent)
        {
            string text = null;
            string firstName = string.Empty;
            bool connected = false;
            try
            {
                var authClient = new LiveAuthClient();
                LiveLoginResult result = silent ? await authClient.InitializeAsync(_defaultAuthScopes) : await authClient.LoginAsync(_defaultAuthScopes);

                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    connected = true;
                    var connectClient = new LiveConnectClient(result.Session);
                    var meResult = await connectClient.GetAsync("me");
                    dynamic meData = meResult.Result;
                    firstName = meData.first_name;

                    await LoadProfileImage(connectClient);
                }
            }
            catch (LiveAuthException ex)
            {
                text = "Error: " + ex.Message;
            }
            catch (LiveConnectException ex)
            {
                text = "Error: " + ex.Message;
            }

            if (text != null)
            {
                var dialog = new Windows.UI.Popups.MessageDialog(text);
                await dialog.ShowAsync();
            }

            UpdateUI(connected, firstName);
        }

        private void apiLabButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ApiLab));
        }

        private void uploadDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UploadDownload));
        }
    }
}
