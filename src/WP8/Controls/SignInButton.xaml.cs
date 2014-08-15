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

namespace Microsoft.Live.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// This partial class contains Windows Phone specific code for the SignInButton.
    /// </summary>
    public partial class SignInButton : Button
    {
        #region Fields

        public static readonly DependencyProperty BrandingProperty = DependencyProperty.Register(
                "Branding",
                typeof(BrandingType),
                typeof(SignInButton),
                new PropertyMetadata(default(BrandingType)));

        public static readonly DependencyProperty ButtonTextTypeProperty = DependencyProperty.Register(
                "ButtonTextType",
                typeof(ButtonTextType),
                typeof(SignInButton),
                new PropertyMetadata(default(ButtonTextType)));

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(
                "ButtonText",
                typeof(string),
                typeof(SignInButton),
                new PropertyMetadata(ResourceHelper.GetResourceString("SignIn")));

        public static readonly DependencyProperty ClientIdProperty = DependencyProperty.Register(
                "ClientId",
                typeof(string),
                typeof(SignInButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ScopesProperty = DependencyProperty.Register(
                "Scopes",
                typeof(string),
                typeof(SignInButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SignInTextProperty = DependencyProperty.Register(
                "SignInText",
                typeof(string),
                typeof(SignInButton),
                new PropertyMetadata(ResourceHelper.GetResourceString("SignIn")));

        public static readonly DependencyProperty SignOutTextProperty = DependencyProperty.Register(
                "SignOutText",
                typeof(string),
                typeof(SignInButton),
                new PropertyMetadata(ResourceHelper.GetResourceString("SignOut")));

        public static readonly DependencyProperty IconTextProperty = DependencyProperty.Register(
            "IconText",
            typeof (string),
            typeof (SignInButton),
            new PropertyMetadata(SkyDriveIcon));

        // Icons are stored characters in the LiveSymbol.ttf
        private const string SkyDriveIcon = "\uE180";
        private const string OutlookIcon = "\uE181";
        private const string MessengerIcon = "\uE182";
        private const string MicrosoftAccountIcon = "\uE183";

        private bool hasPendingLoginRequest;

        #endregion

        #region Properties & Events

        /// <summary>
        /// Gets the branding type of the button. This affects the icon shown in the button.
        /// </summary>
        public BrandingType Branding
        {
            get
            {
                return (BrandingType)this.GetValue(BrandingProperty);
            }

            set
            {
                this.SetValue(BrandingProperty, value);
                this.SetButtonImage();
            }
        }

        /// <summary>
        /// Gets the client id of the application.
        /// </summary>
        public string ClientId
        {
            get
            {
                return this.GetValue(ClientIdProperty) as string;
            }

            set
            {
                this.SetValue(ClientIdProperty, value);
            }
        }

        /// <summary>
        /// Gets the scopes the application needs user consent for.
        /// </summary>
        public string Scopes
        {
            get
            {
                return this.GetValue(ScopesProperty) as string;
            }

            set
            {
                this.SetValue(ScopesProperty, value);
            }
        }

        /// <summary>
        /// Gets button text type. This affects the text shown on the button.
        /// </summary>
        public ButtonTextType TextType
        {
            get
            {
                return (ButtonTextType)this.GetValue(ButtonTextTypeProperty);
            }

            set
            {
                this.SetValue(ButtonTextTypeProperty, value);
                this.SetButtonText();
            }
        }

        /// <summary>
        /// Gets custom sign in text.
        /// </summary>
        public string SignInText
        {
            get
            {
                return this.GetValue(SignInTextProperty) as string;
            }

            set
            {
                this.SetValue(SignInTextProperty, value);
                this.SetButtonText();
            }
        }

        /// <summary>
        /// Gets custom sign out text.
        /// </summary>
        public string SignOutText
        {
            get
            {
                return this.GetValue(SignOutTextProperty) as string;
            }

            set
            {
                this.SetValue(SignOutTextProperty, value);
                this.SetButtonText();
            }
        }

        /// <summary>
        /// Gets whether or not the we're in design mode.
        /// </summary>
        private bool IsDesignMode
        {
            get
            {
                return DesignerProperties.IsInDesignTool;
            }
        }

        #endregion

        #region Methods

        private async void Initialize()
        {
            if (this.authClient == null)
            {
                this.authClient = new LiveAuthClient(this.ClientId);
                if (string.IsNullOrEmpty(this.Scopes))
                {
                    this.Scopes = SignInOfferName;
                }

                IEnumerable<string> scopes = SignInButton.ParseScopeString(this.Scopes);

                try
                {

                    Task<LiveLoginResult> result = this.authClient.InitializeAsync(scopes);

                    LiveLoginResult r = await result;


                    this.OnLogin(r);
                }
                catch (Exception exception)
                {
                    this.RaiseSessionChangedEvent(new LiveConnectSessionChangedEventArgs(exception));
                }

                this.IsEnabled = true;
            }
        }

        private void LoadControlTemplate()
        {
            this.InitializeComponent();
        }

        private async void OnClick(object sender, RoutedEventArgs e)
        {
            if (this.authClient == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentUICulture, 
                        ResourceHelper.GetErrorString("ControlNotInitialized"), 
                        string.Empty, 
                        "Initialize"));
            }

            if (this.currentState == ButtonState.LogIn)
            {
                // if there is a pending login request perform a no-op
                if (this.hasPendingLoginRequest)
                {
                    return;
                }

                this.hasPendingLoginRequest = true;
                try
                {
                    LiveLoginResult result = await this.authClient.LoginAsync(SignInButton.ParseScopeString(this.Scopes));
                    this.OnLogin(result);
                }
                catch (Exception exception)
                {
                    this.RaiseSessionChangedEvent(new LiveConnectSessionChangedEventArgs(exception));
                }
                finally
                {
                    this.hasPendingLoginRequest = false;
                }
            }
            else
            {
                this.authClient.Logout();

                this.Session = null;

                this.SetButtonState(ButtonState.LogIn);

                this.RaiseSessionChangedEvent(
                    new LiveConnectSessionChangedEventArgs(LiveConnectSessionStatus.Unknown, this.Session));
            }
        }

        private void OnControlLoaded(object sender, RoutedEventArgs args)
        {
            this.Initialize();
        }

        private void OnLogin(LiveLoginResult loginResult)
        {
            this.Session = loginResult.Session;

            var sessionChangedArgs = 
                new LiveConnectSessionChangedEventArgs(loginResult.Status, loginResult.Session);

            this.RaiseSessionChangedEvent(sessionChangedArgs);

            if (loginResult.Status == LiveConnectSessionStatus.Connected)
            {
                this.SetButtonState(ButtonState.LogOut);
            }
        }

        private void SetButtonImage()
        {
            string iconText;
            switch (this.Branding)
            {
                case BrandingType.Outlook:
                    iconText = OutlookIcon;
                    break;

                case BrandingType.MicrosoftAccount:
                    iconText = MicrosoftAccountIcon;
                    break;

                case BrandingType.Messenger:
                    iconText = MessengerIcon;
                    break;

                default:
                    iconText = SkyDriveIcon;
                    break;
            }

            this.SetValue(IconTextProperty, iconText);
        }

        #endregion
    }
}
