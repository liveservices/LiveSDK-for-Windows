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

namespace Microsoft.Live
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;

    using Microsoft.Phone.Controls;

    internal class LoginPage : UserControl
    {
        private readonly Uri consentUrl;
        private readonly Uri endUrl;
        private readonly Action<string, Exception> callback;
        private readonly PhoneApplicationFrame rootVisual;
        private readonly PhoneApplicationPage rootPage;
        private readonly WebBrowser webBrowser;
        private readonly ProgressBar progressBar;

        public LoginPage(string consentUrl, string endUrl, Action<string, Exception> callback)
        {
            double rootHeight = Application.Current.RootVisual.RenderSize.Height;
            double rootWidth = Application.Current.RootVisual.RenderSize.Width;

            this.FontFamily = Application.Current.Resources["PhoneFontFamilyNormal"] as FontFamily;
            this.FontSize = (double)Application.Current.Resources["PhoneFontSizeNormal"];
            this.Foreground = Application.Current.Resources["PhoneForegroundBrush"] as Brush;
            this.Background = Application.Current.Resources["PhoneBackgroundBrush"] as Brush;
            this.Height = rootHeight;
            this.Width = rootWidth;

            var root = new Grid()
            {
                Height = rootHeight,
                Width = rootWidth,
                Background = Application.Current.Resources["PhoneBackgroundBrush"] as Brush
            };

            this.progressBar = new ProgressBar()
            {
                Height = 40,
                Width = 460,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Visible,
                IsIndeterminate = true
            };

            this.webBrowser = new WebBrowser()
            {
                Visibility = Visibility.Collapsed,
                Height = rootHeight,
                Width = rootWidth,
                IsScriptEnabled = true
            };

            root.Children.Add(this.progressBar);
            root.Children.Add(this.webBrowser);
            this.Content = root;

            if (string.IsNullOrEmpty(consentUrl))
            {
                throw new ArgumentException("consenUrl");
            }

            try
            {
                this.consentUrl = new Uri(consentUrl, UriKind.Absolute);
            }
            catch (FormatException)
            {
                throw new ArgumentException("consentUrl");
            }

            try
            {
                this.endUrl = new Uri(endUrl, UriKind.Absolute);
            }
            catch (FormatException)
            {
                throw new ArgumentException("endUrl");
            }

            this.callback = callback;
            this.webBrowser.NavigationFailed += OnNavigationFailed;
            this.webBrowser.Navigated += OnFirstNavigated;
            this.webBrowser.Navigating += OnNavigating;
            this.webBrowser.Loaded += OnLoaded;

            this.rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
            if (this.rootVisual != null)
            {
                this.rootPage = this.rootVisual.Content as PhoneApplicationPage;
                if (this.rootPage != null)
                {
                    this.rootPage.BackKeyPress += OnBackKeyPress;
                }
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            this.OnFirstNavigated(this, null);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;
            this.webBrowser.Navigate(this.consentUrl);
        }

        private void OnBackKeyPress(object sender, CancelEventArgs e)
        {
            if (this.webBrowser.CanGoBack)
            {
                this.webBrowser.GoBack();
            }
            else
            {
                this.OnAuthFlowEnded(null);
            }
            e.Cancel = true;
        }

        private void OnFirstNavigated(object sender, NavigationEventArgs args)
        {
            this.progressBar.Visibility = Visibility.Collapsed;
            this.webBrowser.Visibility = Visibility.Visible;
            this.webBrowser.Navigated -= OnFirstNavigated;
        }

        private void OnNavigating(object sender, NavigatingEventArgs args)
        {
            if (args.Uri.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped).Equals(this.endUrl.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped), StringComparison.OrdinalIgnoreCase)
                && args.Uri.AbsolutePath.Equals(this.endUrl.AbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                this.OnAuthFlowEnded(args.Uri.AbsoluteUri);
            }
        }

        private void OnAuthFlowEnded(string resultData)
        {
            if (this.rootPage != null)
            {
                this.rootPage.BackKeyPress -= OnBackKeyPress;
            }

            if (this.callback != null)
            {
                if (!string.IsNullOrEmpty(resultData))
                {
                    this.callback(resultData, null);
                }
                else
                {
                    var error = new LiveAuthException(
                        AuthErrorCodes.AccessDenied, 
                        ResourceHelper.GetString("ConsentNotGranted"));
                    this.callback(null, error);
                }
            }
        }
    }
}