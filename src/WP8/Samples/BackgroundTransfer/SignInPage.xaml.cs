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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;

using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Microsoft.Live.WP8.Samples.BackgroundTransfer
{
    /// <summary>
    /// Sign in page uses the Live SDK Sign In Control to perform the authentication.
    /// Once authenticated it places the retrieved LiveConnectSession in the global Application
    /// class to be used by all the pages.
    /// </summary>
    public partial class SignInPage : PhoneApplicationPage
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void OnSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                ((App) App.Current).Session = e.Session;
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}