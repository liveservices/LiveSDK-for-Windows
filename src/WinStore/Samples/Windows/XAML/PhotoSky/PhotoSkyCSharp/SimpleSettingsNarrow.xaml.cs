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
using System.IO;
using System.Linq;
using PhotoSkyCSharp.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PhotoSkyCSharp
{
    public sealed partial class SimpleSettingsNarrow : UserControl
    {
        public SimpleSettingsNarrow()
        {
            this.InitializeComponent();
            Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator aut = new Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator();
            if (!aut.CanSignOut)
            {
                this.btnSignOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            this.txtUserName.Text = App.UserName;
        }

        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
            
            SettingsPane.Show();
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            SkyDriveDataSource.authClient.Logout();
        }
    }
}
