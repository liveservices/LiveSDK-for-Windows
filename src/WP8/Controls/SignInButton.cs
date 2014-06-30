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
