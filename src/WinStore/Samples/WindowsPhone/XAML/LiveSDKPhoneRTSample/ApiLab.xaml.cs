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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LiveSDKPhoneRTSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApiLab : Page
    {
        public ApiLab()
        {
            this.InitializeComponent();
            PopulateData();  
        }

        private void PopulateData()
        {
            var authScopes = new System.Collections.ObjectModel.ObservableCollection<object>();
            authScopes.Add(new ComboBoxItem() { Content = "wl.signin" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.basic" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.birthday" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.calendars" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.calendars_update" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.contacts_birthday" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.contacts_create" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.contacts_calendars" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.contacts_photos" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.contacts_skydrive" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.emails" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.events_create" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.phone_numbers" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.photos" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.postal_addresses" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.phone_numbersbasic" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.skydrive" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.phone_numbersbasic" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.skydrive_update" });
            authScopes.Add(new ComboBoxItem() { Content = "wl.work_profile" });
            authScopesComboBox.ItemsSource = authScopes;
            authScopesComboBox.SelectedIndex = 0;

            var methods = new System.Collections.ObjectModel.ObservableCollection<object>();
            methods.Add(new ComboBoxItem() { Content = "GET" });
            methods.Add(new ComboBoxItem() { Content = "POST" });
            methods.Add(new ComboBoxItem() { Content = "PUT" });
            methods.Add(new ComboBoxItem() { Content = "DELETE" });
            methods.Add(new ComboBoxItem() { Content = "COPY" });
            methods.Add(new ComboBoxItem() { Content = "MOVE" });
            methodsComboBox.ItemsSource = methods;

            methodsComboBox.SelectedIndex = 0;

            pathTextBox.Text = "me";
         
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
            try
            {
                // Validate parameters
                string path = pathTextBox.Text;
                string destination = destinationTextBox.Text;
                string requestBody = requestBodyTextBox.Text;
                var scope = (authScopesComboBox.SelectedValue as ComboBoxItem).Content as string;
                var method = (methodsComboBox.SelectedValue as ComboBoxItem).Content as string;

                // acquire auth permissions
                var authClient = new LiveAuthClient();
                var authResult = await authClient.LoginAsync(new string[] { scope });
                if (authResult.Session == null)
                {
                    throw new InvalidOperationException("You need to login and give permission to the app.");
                }

                var liveConnectClient = new LiveConnectClient(authResult.Session);
                LiveOperationResult operationResult = null;
                switch (method)
                {
                    case "GET":
                        operationResult = await liveConnectClient.GetAsync(path);
                        break;
                    case "POST":
                        operationResult = await liveConnectClient.PostAsync(path, requestBody);
                        break;
                    case "PUT":
                        operationResult = await liveConnectClient.PutAsync(path, requestBody);
                        break;
                    case "DELETE":
                        operationResult = await liveConnectClient.DeleteAsync(path);
                        break;
                    case "COPY":
                        operationResult = await liveConnectClient.CopyAsync(path, destination);
                        break;
                    case "MOVE":
                        operationResult = await liveConnectClient.MoveAsync(path, destination);
                        break;
                }

                if (operationResult != null)
                {
                    Log("Operation succeeded: \r\n" + operationResult.RawResult);
                }
            }
            catch (Exception ex)
            {
                Log("Got error: " + ex.Message);
            }
        }

        private void Log(string text)
        {
            outputTextBox.Text += text + "\r\n";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            outputTextBox.Text = string.Empty;
        }
    }
}
