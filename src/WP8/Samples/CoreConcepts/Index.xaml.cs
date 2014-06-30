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
using System.Windows;

using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class Index : PhoneApplicationPage
    {
        public Index()
        {
            InitializeComponent();
        }

        private void browseSkyDrive_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/BrowseSkyDrive.xaml", UriKind.Relative));
        }

        private void uploadFile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Upload.xaml", UriKind.Relative));
        }
    }
}