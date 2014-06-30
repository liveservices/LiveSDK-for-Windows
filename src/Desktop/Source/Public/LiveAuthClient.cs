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

namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is designed to help .Net app developers handle user authentication/authorization process.
    /// </summary>
    public class LiveAuthClient : INotifyPropertyChanged
    {
        private readonly LiveAuthClientCore authClient;
        private LiveConnectSession session;
        private bool sessionChanged;
        private SynchronizationContextWrapper syncContext;

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="clientSecret">The client secret of the app.</param>
        public LiveAuthClient(string clientId)
            : this(clientId, null)
        {
        }

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="refreshTokenHandler">An IRefreshTokenHandler instance to handle refresh token persistency and retrieval.</param>
        public LiveAuthClient(
            string clientId,
            IRefreshTokenHandler refreshTokenHandler)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(clientId, "clientId");

            this.authClient = new LiveAuthClientCore(clientId, refreshTokenHandler, this);
            this.syncContext = SynchronizationContextWrapper.Current;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

#if DEBUG
        /// <summary>
        /// Allows the application to override the default auth server host name.
        /// </summary>
        public static string AuthEndpointOverride { get; set; }
#endif

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public LiveConnectSession Session
        {
            get
            {
                return this.session;
            }

            internal set
            {
                if (this.session != value)
                {
                    this.session = value;
                    this.sessionChanged = true;
                }
            }
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance by trying to retrieve an access token using refresh token
        /// provided by the app via the IRefreshTokenHandler instance.
        /// </summary>
        /// <returns>An async Task instance</returns>
        public Task<LiveLoginResult> InitializeAsync()
        {
            return this.InitializeAsync(new string[] { });
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance. 
        /// This will trigger retrieving token with refresh token process if the app provides the refresh token via
        /// IRefreshTokenHandler.RetrieveRefreshTokenAsync method.
        /// </summary>
        /// <param name="scopes">The list of offers that the application is requesting user to consent for.</param>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> InitializeAsync(IEnumerable<string> scopes)
        {
            LiveUtility.ValidateNotNullParameter(scopes, "scopes");
            return this.authClient.InitializeAsync(scopes);
        }

        /// <summary>
        /// Exchange authentication code for access token.
        /// </summary>
        /// <param name="AuthenticationCode">The authentication code the app received from Microsoft authorization
        /// server during the user authorization process.</param>
        /// <returns></returns>
        public Task<LiveConnectSession> ExchangeAuthCodeAsync(string authenticationCode)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(authenticationCode, "authenticationCode");

            return this.authClient.ExchangeAuthCodeAsync(authenticationCode);
        }

        
        /// <summary>
        /// Generates a consent URL that includes a set of provided  parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to authorize.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes)
        {
            return this.GetLoginUrl(scopes, null);
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided  parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to authorize.</param>
        /// <param name="options">A table of optional authorization parameters to be encoded into the URL.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes, IDictionary<string, string> options)
        {
            LiveUtility.ValidateNotEmptyStringEnumeratorArguement(scopes, "scopes");

            string locale = null;
            string state = null;
            DisplayType display = DisplayType.WinDesktop;
            ThemeType theme = ThemeType.Win7;
            string redirectUrl = LiveAuthUtility.BuildDesktopRedirectUrl();

            if (options != null)
            {
                if (options.ContainsKey(AuthConstants.Locale))
                {
                    locale = options[AuthConstants.Locale];
                }

                if (options.ContainsKey(AuthConstants.ClientState))
                {
                    state = options[AuthConstants.ClientState];
                }

                if (options.ContainsKey(AuthConstants.Display))
                {
                    string displayStr = options[AuthConstants.Display];
                    if (!Enum.TryParse<DisplayType>(displayStr, true, out display))
                    {
                        throw new ArgumentException(ErrorText.ParameterInvalidDisplayValue, "display");
                    }
                }

                if (options.ContainsKey(AuthConstants.Theme))
                {
                    string themeStr = options[AuthConstants.Theme];
                    if (!Enum.TryParse<ThemeType>(themeStr, true, out theme))
                    {
                        throw new ArgumentException(ErrorText.ParameterInvalidDisplayValue, "theme");
                    }
                }
            }

            if (locale == null)
            {
                locale = CultureInfo.CurrentUICulture.ToString();
            }

            return this.authClient.GetLoginUrl(scopes, redirectUrl, display, theme, locale, state);
        }

        /// <summary>
        /// Gets the logout URL.
        /// </summary>
        /// <returns>The logout URL.</returns>
        public string GetLogoutUrl()
        {
            return LiveAuthUtility.BuildLogoutUrl();
        }

        /// <summary>
        /// This method is used to ensure that the property changed event is only invoked once during the execution of
        /// InitUserPresentAsync and InitUserAbsentAsync methods.
        /// </summary>
        internal void FirePendingPropertyChangedEvents()
        {
            if (this.sessionChanged)
            {
                this.OnPropertyChanged("Session");
                this.sessionChanged = false;
            }
        }

        internal bool RefreshToken(Action<LiveLoginResult> completionCallback)
        {
            bool noValidSession = (this.session == null || !this.session.IsValid);
            if (noValidSession && this.authClient.CanRefreshToken)
            {
                this.authClient.TryRefreshToken(completionCallback);
                return true;
            }

            return false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                this.syncContext.Post(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
