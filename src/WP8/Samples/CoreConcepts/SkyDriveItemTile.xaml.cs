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
using System.Windows.Controls;

namespace CoreConcepts
{
    public partial class SkyDriveItemTile : UserControl
    {
        public SkyDriveItemTile()
        {
            InitializeComponent();
            this.ButtonItem.Click += ButtonItem_Click;
        }

        public event RoutedEventHandler OnItemSelect;

        public string ItemName
        {
            get
            {
                return this.ButtonItem.Content as string;
            }
            set
            {
                this.ButtonItem.Content = value;
            }
        }

        private void  ButtonItem_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.OnItemSelect;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
