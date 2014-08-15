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
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

using Microsoft.Live;
using Microsoft.Live.Controls;

namespace APIExplorer
{
    /// <summary>
    ///     Main application page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        ///     Stores the LiveConnectClient instance.
        /// </summary>
        private LiveConnectClient liveClient;

        /// <summary>
        ///     Constructor to create the main page.  Initializes all UI components.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "GET" });
            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "POST" });
            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "PUT" });
            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "DELETE" });
            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "MOVE" });
            this.cbMethod.Items.Add(new ComboBoxItem() { Content = "COPY" });
            this.cbMethod.SelectedIndex = 0;
        }

        /// <summary>
        ///     Event handler for SignInButton.SessionChanged event.
        /// </summary>
        private void btnLogin_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                this.liveClient = new LiveConnectClient(e.Session);
                this.GetMe();
            }
            else
            {
                this.liveClient = null;
                this.tbResponse.Text = e.Error != null ? e.Error.ToString() : string.Empty;
                this.imgMe.Source = null;
            }
        }

        /// <summary>
        ///     Retrieves the basic profile information of the current user.
        /// </summary>
        private async void GetMe()
        {
            try
            {
                LiveOperationResult result = await this.liveClient.GetAsync("me");

                dynamic profile = result.Result;
                this.tbWelcome.Text = "Welcome " + profile.first_name + " " + profile.last_name;

                this.GetProfilePicture();

                this.cbMethod.SelectedIndex = 0;
                this.CallApi();
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
        }

        /// <summary>
        ///     Downloads the profile picture of the current user.
        /// </summary>
        private async void GetProfilePicture()
        {
            try
            {
                LiveDownloadOperationResult downloadOperationResult = await this.liveClient.DownloadAsync("me/picture");
                Stream stream = downloadOperationResult.Stream;
                if (stream != null)
                {
                    BitmapImage imgSource = new BitmapImage();
                    imgSource.SetSource(stream);
                    this.imgMe.Source = imgSource;
                }
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
        }

        /// <summary>
        ///     Event handler to handle the ComboBox.SelectionChanged event.
        /// </summary>
        private void cbMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = this.cbMethod.SelectedItem as ComboBoxItem;
            switch (selectedItem.Content as string)
            {
                case "MOVE":
                case "COPY":
                    this.spTo.Visibility = Visibility.Visible;
                    this.tbResponse.Visibility = Visibility.Collapsed;
                    break;

                default:
                    this.spTo.Visibility = Visibility.Collapsed;
                    this.tbResponse.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        ///     Event handler to handle the "Go" button Click event.  
        ///     It kicks off a call to the Live Connect API service.
        /// </summary>
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            this.tbResponse.Text = "Calling api service...";
            this.CallApi();
        }

        /// <summary>
        ///     Call the Live Connect API service based on inputs in the UI elements.
        /// </summary>
        private async void CallApi()
        {
            if (this.liveClient == null)
            {
                this.tbResponse.Text = "Please sign in first.";
                return;
            }

            try
            {
                LiveOperationResult result = null;
                ComboBoxItem selectedItem = this.cbMethod.SelectedItem as ComboBoxItem;
                switch (selectedItem.Content as string)
                {
                    case "GET":
                        result = await this.liveClient.GetAsync(this.tbUrl.Text);
                        break;

                    case "DELETE":
                        result = await this.liveClient.DeleteAsync(this.tbUrl.Text);
                        break;

                    case "POST":
                        result = await this.liveClient.PostAsync(this.tbUrl.Text, this.tbResponse.Text);
                        break;

                    case "PUT":
                        result = await this.liveClient.PutAsync(this.tbUrl.Text, this.tbResponse.Text);
                        break;

                    case "COPY":
                        result = await this.liveClient.CopyAsync(this.tbUrl.Text, this.tbTo.Text);
                        break;

                    case "MOVE":
                        result = await this.liveClient.MoveAsync(this.tbUrl.Text, this.tbTo.Text);
                        break;

                    default:
                        this.tbResponse.Text = "Please select a method to call.";
                        break;
                }

                if (result != null)
                {
                    this.tbResponse.Text = result.RawResult;
                }
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }

            this.cbMethod.SelectedIndex = 0;
            this.tbResponse.Visibility = Visibility.Visible;
            this.spTo.Visibility = Visibility.Collapsed;
        }
    }
}