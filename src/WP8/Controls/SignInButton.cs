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
    using System.Diagnostics;

    /// <summary>
    /// This partial class contains shared code between Windows Phone SignInButton and
    /// Windows 8 SignInButton.
    /// </summary>
    public partial class SignInButton
    {
        #region Private Members

        private static readonly char[] ScopeSeparators = new char[] { ' ', ',' };
        private const string SignInOfferName = "wl.signin";

        private ButtonState currentState;
        private LiveAuthClient authClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new SignInButton.
        /// </summary>
        public SignInButton()
        {
            this.LoadControlTemplate();
            this.SetButtonState(ButtonState.LogIn);

            if (!this.IsDesignMode)
            {
                this.IsEnabled = false;
                this.Loaded += OnControlLoaded;
                this.Click += OnClick;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the session object.
        /// </summary>
        public LiveConnectSession Session { get; internal set; }

        #endregion

        #region Events

        /// <summary>
        /// Event that fires when the session object changes.
        /// </summary>
        public event EventHandler<LiveConnectSessionChangedEventArgs> SessionChanged;

        #endregion

        #region Methods

        private static IEnumerable<string> ParseScopeString(string scopesString)
        {
            Debug.Assert(!string.IsNullOrEmpty(scopesString));

            return new List<string>(
                scopesString.Split(SignInButton.ScopeSeparators, StringSplitOptions.RemoveEmptyEntries));
        }

        private void RaiseSessionChangedEvent(LiveConnectSessionChangedEventArgs args)
        {
            var handler = this.SessionChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void SetButtonState(ButtonState newState)
        {
            this.currentState = newState;

            this.SetButtonText();

            this.SetButtonImage();
        }

        private void SetButtonText()
        {
            string currentText = this.GetButtonText();
            this.SetValue(ButtonTextProperty, currentText);
        }

        private string GetButtonText()
        {
            bool isLogout = (this.currentState == ButtonState.LogOut);
            string buttonText = string.Empty;
            switch (this.TextType)
            {
                case ButtonTextType.SignIn:
                    buttonText = ResourceHelper.GetResourceString(isLogout ? "SignOut" : "SignIn");
                    break;

                case ButtonTextType.Login:
                    buttonText = ResourceHelper.GetResourceString(isLogout ? "Logout" : "Login");
                    break;

                case ButtonTextType.Connect:
                    buttonText = ResourceHelper.GetResourceString(isLogout ? "Logout" : "Connect");
                    break;

                case ButtonTextType.Custom:
                    if (!isLogout)
                    {
                        if (string.IsNullOrEmpty(this.SignInText))
                        {
                            this.SignInText = ResourceHelper.GetResourceString("SignIn");
                        }

                        buttonText = this.SignInText;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(this.SignOutText))
                        {
                            this.SignOutText = ResourceHelper.GetResourceString("SignOut");
                        }

                        buttonText = this.SignOutText;
                    }

                    break;

                default:
                    Debug.Assert(false, "unknown ButtonTextType.");
                    break;
            }

            return buttonText;
        }

        #endregion

        #region ButtonState enum

        private enum ButtonState
        {
            LogIn,
            LogOut
        }

        #endregion
    }
}
