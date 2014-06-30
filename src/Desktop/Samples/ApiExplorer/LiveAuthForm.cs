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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Live.Desktop.Samples.ApiExplorer
{
    public delegate void AuthCompletedCallback(AuthResult result);

    public partial class LiveAuthForm : Form
    {
        private readonly string startUrl;
        private readonly string endUrl;
        private readonly AuthCompletedCallback callback;

        public LiveAuthForm(string startUrl, string endUrl, AuthCompletedCallback callback)
        {
            this.startUrl = startUrl;
            this.endUrl = endUrl;
            this.callback = callback;
            InitializeComponent();
        }

        private void LiveAuthForm_Load(object sender, EventArgs e)
        {
            this.webBrowser.Navigated += WebBrowser_Navigated;
            this.webBrowser.Navigate(this.startUrl);

        }

        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (this.webBrowser.Url.AbsoluteUri.StartsWith(this.endUrl))
            {
                if (this.callback != null)
                {
                    this.callback(new AuthResult(this.webBrowser.Url));
                }
            }
        }
    }
}
