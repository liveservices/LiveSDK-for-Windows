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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    /// This class is designed to help web applications developers handle user authentication/authorization process.
    /// </summary>
    public class LiveAuthClient : INotifyPropertyChanged
    {
        private readonly LiveAuthClientCore authClient;
        private readonly string defaultRedirectUrl;
        private LiveConnectSession session;
        private string currentUserId;
        private bool sessionChanged;
        private bool currentUserIdChanged;

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="clientSecret">The client secret of the app.</param>
        /// <param name="defaultRedirectUrl">The default redirect URL for the site.</param>
        public LiveAuthClient(
            string clientId,
            string clientSecret,
            string defaultRedirectUrl)
            : this(clientId, clientSecret, defaultRedirectUrl, null)
        {
        }

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="clientSecret">The client secret of the app.</param>
        /// <param name="defaultRedirectUrl">The default redirect URL for the site.</param>
        /// <param name="refreshTokenHandler">An IRefreshTokenHandler instance to handle refresh token persistency and retrieval.</param>
        public LiveAuthClient(
            string clientId,
            string clientSecret,
            string defaultRedirectUrl,
            IRefreshTokenHandler refreshTokenHandler)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(clientId, "clientId");
            LiveUtility.ValidateNotNullOrWhiteSpaceString(clientSecret, "clientSecret");
            if (!string.IsNullOrWhiteSpace(defaultRedirectUrl))
            {
                LiveUtility.ValidateUrl(defaultRedirectUrl, "defaultRedirectUrl");
                this.defaultRedirectUrl = defaultRedirectUrl;
            }

            this.authClient = new LiveAuthClientCore(clientId, clientSecret, refreshTokenHandler, this);
        }

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// To address security concerns, developers may wish to change their client secret. This constructor allows
        /// the developer to pass in a client secret map to handle the client secret change and ensure a graceful experience
        /// during the change rollover. You can create a new secret for your app on the developer portal (https://account.live.com/developers/applications)
        /// site. Once a new secret is created, the site will indicate the version of each secret, new and old.
        /// For example, the old secret and new secret may be shown as v0 and v1 on the developer portal site.
        /// Before the new secret is activated, both secrets are accepted by the Microsoft authentication server. You should update your 
        /// code to provide the secret map when intializing a LiveAuthClient instance. This will ensure that the new secret
        /// will be used when communicating with the Microsoft authentication server, and the LiveAuthClient instance will use the correct version of
        /// the client secret to validate authentication tokens that may be signed by either old or new versions of client secret. 
        /// Once you activate the new secret on the developer portal site, the old secret will no longer be accepted by the Microsoft 
        /// authentication server, however, you may still want to keep the old secret in the map for one day, so that 
        /// clients with authentication tokens signed with the old secret will continue to work.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="clientSecretMap">The client secret map of the app.</param>
        /// <param name="defaultRedirectUrl">The default redirect URL for the site.</param>
        public LiveAuthClient(
            string clientId,
            IDictionary<int, string> clientSecretMap,
            string defaultRedirectUrl)
            : this(clientId, clientSecretMap, defaultRedirectUrl, null)
        {
        }

        /// <summary>
        /// Initializes an instance of LiveAuthClient class.
        /// To address security concerns, developers may wish to change their client secret. This constructor allows
        /// the developer to pass in a client secret map to handle the client secret change and ensure a graceful experience
        /// during the change rollover. You can create a new secret for your app on the developer portal (https://account.live.com/developers/applications)
        /// site. Once a new secret is created, the site will indicate the version of each secret, new and old.
        /// For example, the old secret and new secret may be shown as v0 and v1 on the developer portal site.
        /// Before the new secret is activated, both secrets are accepted by the Microsoft authentication server. You should update your 
        /// code to provide the secret map when intializing a LiveAuthClient instance. This will ensure that the new secret
        /// will be used when communicating with the Microsoft authentication server, and the LiveAuthClient instance will use the correct version of
        /// the client secret to validate authentication tokens that may be signed by either old or new versions of client secret. 
        /// Once you activate the new secret on the developer portal site, the old secret will no longer be accepted by the Microsoft 
        /// authentication server, however, you may still want to keep the old secret in the map for one day, so that 
        /// clients with authentication tokens signed with the old secret will continue to work.
        /// </summary>
        /// <param name="clientId">The client Id of the app.</param>
        /// <param name="clientSecretMap">The client secret map of the app.</param>
        /// <param name="defaultRedirectUrl">The default redirect URL for the site.</param>
        /// <param name="refreshTokenHandler">An IRefreshTokenHandler instance to handle refresh token persistency and retrieval.</param>
        public LiveAuthClient(
            string clientId,
            IDictionary<int, string> clientSecretMap,
            string defaultRedirectUrl,
            IRefreshTokenHandler refreshTokenHandler)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(clientId, "clientId");
            LiveUtility.ValidateNotNullParameter(clientSecretMap, "clientSecretMap");
            if (clientSecretMap.Count == 0)
            {
                throw new ArgumentException("Client secret must be provided.", "clientSecretMap");
            }

            if (!string.IsNullOrWhiteSpace(defaultRedirectUrl))
            {
                LiveUtility.ValidateUrl(defaultRedirectUrl, "defaultRedirectUrl");
                this.defaultRedirectUrl = defaultRedirectUrl;
            }

            this.authClient = new LiveAuthClientCore(clientId, clientSecretMap, refreshTokenHandler, this);   
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
                    this.UpdateCurrentUseId();
                    this.sessionChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets the current user's Id.
        /// </summary>
        public string CurrentUserId
        {
            get
            {
                return this.currentUserId;
            }

            private set
            {
                if (this.currentUserId != value)
                {
                    this.currentUserId = value;
                    this.currentUserIdChanged = true;
                }
            }
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance for the current user. 
        /// If an authorization code is present, it will send a request to the auth server to exchange the token.
        /// If there is an auth session in current context, it will retrieve the current auth session.
        /// If the current session is expired or the current request url indicates (refresh=1) to get a new token, 
        /// it will try to request a new access token using refresh token that will need to be provided through 
        /// IRefreshTokenHandler.RetrieveRefreshTokenAsync() method.
        /// Any updated session state will be saved in the auth session cookie.
        /// </summary>
        /// <param name="context">The HttpContextBase instance of current request.</param>
        /// <returns>An async Task instance</returns>
        public Task<LiveLoginResult> InitializeWebSessionAsync(
            HttpContextBase context)
        {
            return this.InitializeWebSessionAsync(context, this.defaultRedirectUrl);
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance for the current user. 
        /// If an authorization code is present, it will send a request to the auth server to exchange the token.
        /// If there is an auth session in current context, it will retrieve the current auth session.
        /// If the current session is expired or the current request url indicates (refresh=1) to get a new token, 
        /// it will try to request a new access token using refresh token that will need to be provided through 
        /// IRefreshTokenHandler.RetrieveRefreshTokenAsync() method.
        /// Any updated session state will be saved in the auth session cookie.
        /// </summary>
        /// <param name="context">The HttpContextBase instance of current request.</param>
        /// <param name="redirectUrl">The redirect URL of the app. This must match exactly the Url that is used to 
        /// generate the login Url via LiveAuthClient.GetLoginUrl</param>
        /// <returns>An async Task instance</returns>
        public Task<LiveLoginResult> InitializeWebSessionAsync(
            HttpContextBase context,
            string redirectUrl)
        {
            return this.InitializeWebSessionAsync(context, redirectUrl, null);
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance for the current user. 
        /// If an authorization code is present, it will send a request to the auth server to exchange the token.
        /// If there is an auth session in current context, it will retrieve the current auth session.
        /// If the current session is expired or the current request url indicates (refresh=1) to get a new token, 
        /// it will try to request a new access token using refresh token that will need to be provided through 
        /// IRefreshTokenHandler.RetrieveRefreshTokenAsync() method.
        /// Any updated session state will be saved in the auth session cookie.
        /// </summary>
        /// <param name="context">The HttpContextBase instance of current request.</param>
        /// <param name="redirectUrl">The redirect URL of the app. This must match exactly the Url that is used to 
        /// generate the login Url via LiveAuthClient.GetLoginUrl</param>
        /// <param name="scopes">A list of scopes to validate whether the user has consented. If the available session 
        /// does not satisfy the specified scopes, NotConnected status will be returned. However, the developer still
        /// can find the available session throw the Session property.</param>
        /// <returns>An async Task instance</returns>
        public Task<LiveLoginResult> InitializeWebSessionAsync(
            HttpContextBase context,
            string redirectUrl,
            IEnumerable<string> scopes)
        {
            LiveUtility.ValidateNotNullParameter(context, "context");
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = LiveAuthUtility.GetCurrentRedirectUrl(context.Request.Url);
            }
            else
            {
                LiveUtility.ValidateUrl(redirectUrl, "redirectUrl");
            }
            
            return this.authClient.InitializeWebSessionAsync(redirectUrl, context, scopes);
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance. 
        /// This will trigger retrieving token with refresh token process.
        /// </summary>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> InitializeSessionAsync()
        {
            if (this.defaultRedirectUrl == null)
            {
                throw new InvalidOperationException(ErrorText.DefaultRedirectUrlMissing);
            }

            return this.authClient.InitializeSessionAsync(this.defaultRedirectUrl, null);
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance. 
        /// This will trigger retrieving token with refresh token process.
        /// </summary>
        /// <param name="redirectUri">The redirect URL of the app.</param>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> InitializeSessionAsync(string redirectUrl)
        {
            return this.InitializeSessionAsync(redirectUrl, null);
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance. 
        /// This will trigger retrieving token with refresh token process.
        /// </summary>
        /// <param name="redirectUri">The redirect URL of the app.</param>
        /// <param name="scopes">A list of scopes to validate whether the user has consented.</param>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> InitializeSessionAsync(string redirectUrl, IEnumerable<string> scopes)
        {
            LiveUtility.ValidateUrl(redirectUrl, "redirectUrl");

            return this.authClient.InitializeSessionAsync(redirectUrl ,scopes);
        }

        /// <summary>
        /// Exchange authentication code from the Http request for access token.
        /// </summary>
        /// <param name="context">The HttpContextBase instance</param>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> ExchangeAuthCodeAsync(HttpContextBase context)
        {
            return this.ExchangeAuthCodeAsync(context, this.defaultRedirectUrl);
        }

        /// <summary>
        /// Exchange authentication code for access token.
        /// </summary>
        /// <param name="context">The HttpContextBase instance</param>
        /// <param name="redirectUri">The redirect URL of the app.</param>
        /// <returns>An async Task instance.</returns>
        public Task<LiveLoginResult> ExchangeAuthCodeAsync(
            HttpContextBase context,
            string redirectUrl)
        {
            LiveUtility.ValidateNotNullParameter(context, "context");

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = LiveAuthUtility.GetCurrentRedirectUrl(context.Request.Url);
            }
            else
            {
                LiveUtility.ValidateUrl(redirectUrl, "redirectUrl");
            }

            return this.authClient.ExchangeAuthCodeAsync(redirectUrl, context);
        }
        
        /// <summary>
        /// Clear the auth state in the current session
        /// </summary>
        /// <param name="context">Optional</param>
        public void ClearSession(HttpContextBase context)
        {
            LiveUtility.ValidateNotNullParameter(context, "context");

            this.authClient.ClearSession(context);
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to consent to.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes)
        {
            if (this.defaultRedirectUrl == null)
            {
                throw new InvalidOperationException(ErrorText.DefaultRedirectUrlMissing);
            }

            return this.GetLoginUrl(scopes, this.defaultRedirectUrl);
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to consent to.</param>
        /// <param name="redirectUri">The URL that the page will be redirected to after authorize process is completed.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes, string redirectUrl)
        {
            return this.GetLoginUrl(scopes, redirectUrl, null);
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to authorize.</param>
        /// <param name="options">Optional query string parameters.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes, IDictionary<string, string> options)
        {
            if (this.defaultRedirectUrl == null)
            {
                throw new InvalidOperationException(ErrorText.DefaultRedirectUrlMissing);
            }

            return this.GetLoginUrl(scopes, this.defaultRedirectUrl, options);
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided parameters.
        /// </summary>
        /// <param name="scopes">A list of scope values that the user will need to authorize.</param>
        /// <param name="redirectUri">The URL that the page will be redirected to after authorize process is completed.</param>
        /// <param name="options">Optional query string parameters.</param>
        /// <returns>The generated login URL value.</returns>
        public string GetLoginUrl(IEnumerable<string> scopes, string redirectUrl, IDictionary<string, string> options)
        {
            LiveUtility.ValidateNotEmptyStringEnumeratorArguement(scopes, "scopes");
            LiveUtility.ValidateUrl(redirectUrl, "redirectUrl");

            string locale = null;
            string state = null;
            DisplayType display = DisplayType.Page;

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
            }

            if (locale == null)
            {
                locale = CultureInfo.CurrentUICulture.ToString();
            }


            return this.authClient.GetLoginUrl(scopes, redirectUrl, display, locale, state);
        }

        /// <summary>
        /// Gets the logout URL.
        /// </summary>
        /// <param name="redirectUri">The URL that the page will be redirected to after logout is completed.</param>
        /// <returns>The logout URL.</returns>
        public string GetLogoutUrl(string redirectUrl)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(redirectUrl, "redirectUrl");
            
            return this.authClient.GetLogoutUrl(redirectUrl);
        }

        /// <summary>
        /// Decrypts the user ID from a given authentication token.
        /// </summary>
        /// <returns>The user ID string value</returns>
        public string GetUserId(string authenticationToken)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(authenticationToken, "authenticationToken");

            string userId;
            LiveAuthException error;
            if (this.authClient.GetUserId(authenticationToken, out userId, out error))
            {
                return userId;
            }
            else
            {
                throw error;
            }
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

            if (this.currentUserIdChanged)
            {
                this.OnPropertyChanged("CurrentUserId");
                this.currentUserIdChanged = false;
            }
        }

        internal bool RefreshToken(Action<LiveLoginResult> completionCallback)
        {
            // This scenario is not supported in the Web, because refreshing ticket 
            // should only happen when InitializeWebSessionAsync or InitializeSessionAsync is 
            // invoked.
            return false;
        }

        private void UpdateCurrentUseId()
        {
            string userId = null;
            if (this.session != null)
            {
                string authenticationToken = this.session.AuthenticationToken;
                if (!string.IsNullOrEmpty(authenticationToken))
                {
                    LiveAuthException error;
                    this.authClient.GetUserId(authenticationToken, out userId, out error);
                }
            }

            this.CurrentUserId = userId;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
