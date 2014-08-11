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
using System.Threading.Tasks;

using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace APIExplorer
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly string[] scopes = new string[] { "wl.signin", "wl.basic", "wl.skydrive_update" };
        private LiveAuthClient authClient;
        private LiveConnectClient liveClient;

        public MainPage()
        {
            this.InitializeComponent();

            this.cbMethod.Items.Add("SELECT");
            this.cbMethod.Items.Add("GET");
            this.cbMethod.Items.Add("POST");
            this.cbMethod.Items.Add("PUT");
            this.cbMethod.Items.Add("DELETE");
            this.cbMethod.Items.Add("MOVE");
            this.cbMethod.Items.Add("COPY");

            this.cbMethod.SelectedIndex = 1;

            this.tbResponse.Text = string.Empty;
            this.tbResponse.MaxLength = int.MaxValue;
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
                    this.GetMe();
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
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
                        this.GetMe();
                    }
                }
                else
                {
                    this.authClient.Logout();
                    this.btnLogin.Content = "Sign In";
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
            }
        }

        private async void GetMe()
        {
            try
            {
                Task<LiveOperationResult> task = this.liveClient.GetAsync("me");

                var result = await task;
                dynamic profile = result.Result;
                this.tbWelcome.Text = "Welcome " + profile.first_name + " " + profile.last_name;

                this.GetProfilePicture();

                this.cbMethod.SelectedIndex = 1;
                this.CallApi();
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
            catch (OperationCanceledException)
            {
                this.tbResponse.Text = "Operation cancelled.";
            }
        }

        private async void GetProfilePicture()
        {
            try
            {
                LiveDownloadOperation operation = await this.liveClient.CreateBackgroundDownloadAsync("me/picture");
                LiveDownloadOperationResult result = await operation.StartAsync();
                if (result != null && result.Stream != null)
                {
                    using (IRandomAccessStream ras = await result.GetRandomAccessStreamAsync())
                    {
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.SetSource(ras);
                        this.imgMe.Source = imgSource;
                    }
                }
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
        }

        private void cbMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((string)this.cbMethod.SelectedItem))
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

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            this.tbResponse.Text = "Calling api service...";
            this.CallApi();
        }

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
                switch ((string)this.cbMethod.SelectedItem)
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
