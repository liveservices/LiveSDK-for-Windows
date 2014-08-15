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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;

using Microsoft.Live;

namespace LiveLogin
{

    /// <summary>
    ///     Main application page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        ///     Defines the scopes the application needs.
        /// </summary>
        private static readonly string[] scopes = new string[] { "wl.signin", "wl.basic", "wl.offline_access" };

        /// <summary>
        ///     Stores the LiveAuthClient instance.
        /// </summary>
        private LiveAuthClient authClient;

        /// <summary>
        ///     Stores the LiveConnectClient instance.
        /// </summary>
        private LiveConnectClient liveClient;

        /// <summary>
        ///     Constructor to create the main page.  
        ///     Initializes all UI components.
        ///     Initializes user login status.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            this.InitializePage();
        }

        /// <summary>
        ///     Calls LiveAuthClient.Initialize to get the user login status.
        ///     Retrieves user profile information if user is already signed in.
        /// </summary>
        private async void InitializePage()
        {
            try
            {
                this.authClient = new LiveAuthClient("0000000048087CFD");
                LiveLoginResult loginResult = await this.authClient.InitializeAsync(scopes);
                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    this.btnLogin.Content = "Sign Out";

                    this.liveClient = new LiveConnectClient(loginResult.Session);
                    this.GetMe();
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
            }
        }

        /// <summary>
        ///     Event handler called when the login button is clicked.
        /// </summary>
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.btnLogin.Content.ToString() == "Sign In")
                {
                    LiveLoginResult loginResult = await this.authClient.LoginAsync(scopes);
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        this.btnLogin.Content = "Sign Out";

                        this.liveClient = new LiveConnectClient(loginResult.Session);
                        this.GetMe();
                    }
                }
                else
                {
                    this.authClient.Logout();
                    this.btnLogin.Content = "Sign In";
                    this.tbResponse.Text = "";
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
            }
        }

        /// <summary>
        ///     Retrieves the user profile information for the Live Connect API service.
        /// </summary>
        private async void GetMe()
        {
            try
            {
                LiveOperationResult operationResult = await this.liveClient.GetAsync("me");

                dynamic properties = operationResult.Result;
                this.tbResponse.Text = properties.first_name + " " + properties.last_name;
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
        }
    }
}