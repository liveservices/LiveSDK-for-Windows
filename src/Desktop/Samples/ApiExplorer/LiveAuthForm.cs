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
