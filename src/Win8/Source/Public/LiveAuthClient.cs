namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Security.Authentication.Web;

    using Microsoft.Live.Operations;

    public partial class LiveAuthClient
    {
        private const string SignInOfferName = "wl.signin";
        private List<string> currentScopes;
        private ThemeType? theme;

        #region Constructor

        /// <summary>
        /// Creates a new instance of the auth client.  Takes no parameters. 
        /// The application client id is the same as the application package sid.
        /// </summary>
        public LiveAuthClient()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new instance of the auth client. 
        /// The application client id is the same as the application package sid.
        /// </summary>
        /// <param name="redirectUri">The application's redirect uri as specified in application management portal.</param>
        public LiveAuthClient(string redirectUri)
        {
            if (!string.IsNullOrEmpty(redirectUri) && !IsValidRedirectDomain(redirectUri))
            {
                throw new ArgumentException(
                    redirectUri,
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), "redirectUri"));
            }

            this.AuthClient = new TailoredAuthClient(this);

            string appId = this.GetAppPackageSid();
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = appId;
            }

            this.InitializeMembers(appId.TrimEnd('/'), redirectUri);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets and sets the theme used for the consent request.
        /// </summary>
        public ThemeType Theme
        {
            get
            {
                if (!this.theme.HasValue)
                {
                    this.theme = Platform.GetThemeType();
                }

                return this.theme.Value;
            }
            set
            {
                this.theme = value;
            }
        }

        /// <summary>
        /// Gets whether or not sign out is supported for the logged in user.
        /// </summary>
        /// <remarks>Sign out is only supported for non-connected user accounts.</remarks>
        public bool CanLogout
        {
            get
            {
                return this.AuthClient.CanSignOut;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the auth client. Detects if user is already signed in,
        /// If user is already signed in, creates a valid Session.
        /// This call is UI-less.
        /// </summary>
        public Task<LiveLoginResult> InitializeAsync()
        {
            return this.InitializeAsync(new List<string>());
        }
        
        /// <summary>
        /// Initializes the auth client. Detects if user is already signed in,
        /// If user is already signed in, creates a valid Session.
        /// This call is UI-less.
        /// </summary>
        /// <param name="scopes">The list of offers that the application is requesting user consent for.</param>
        public Task<LiveLoginResult> InitializeAsync(IEnumerable<string> scopes)
        {
            if (scopes == null)
            {
                throw new ArgumentNullException("scopes");
            }

            return this.ExecuteAuthTaskAsync(scopes, true);
        }

        /// <summary>
        /// Displays the login/consent UI and returns a Session object when user completes the auth flow.
        /// </summary>
        /// <param name="scopes">The list of offers that the application is requesting user consent for.</param>
        public Task<LiveLoginResult> LoginAsync(IEnumerable<string> scopes)
        {
            if (scopes == null && this.scopes == null)
            {
                throw new ArgumentNullException("scopes");
            }
            
            return this.ExecuteAuthTaskAsync(scopes, false);
        }

        /// <summary>
        /// Logs user out of the application.  Clears any cached Session data.
        /// </summary>
        public void Logout()
        {
            if (!this.CanLogout)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("CantLogout"));
            }

            this.AuthClient.CloseSession();
        }

        #endregion

        #region Internal/Private Methods

        /// <summary>
        /// Creates a LiveConnectSession object based on the parsed response.
        /// </summary>
        internal static LiveConnectSession CreateSession(LiveAuthClient client, IDictionary<string, object> result)
        {
            var session = new LiveConnectSession(client);

            Debug.Assert(result.ContainsKey(AuthConstants.AccessToken));
            if (result.ContainsKey(AuthConstants.AccessToken))
            {
                session.AccessToken = result[AuthConstants.AccessToken] as string;

                if (result.ContainsKey(AuthConstants.AuthenticationToken))
                {
                    session.AuthenticationToken = result[AuthConstants.AuthenticationToken] as string;
                }
            }

            return session;
        }

        /// <summary>
        /// Retrieve a new access token based on current session information.
        /// </summary>
        internal bool RefreshToken(Action<LiveLoginResult> completionCallback)
        {
            this.TryRefreshToken(completionCallback);

            return true;
        }

        private async Task<LiveLoginResult> ExecuteAuthTaskAsync(IEnumerable<string> scopes, bool silent)
        {
            if (scopes != null)
            {
                this.scopes = new List<string>(scopes);
            }

            this.EnsureSignInScope();
            this.PrepareForAsync();

            LiveLoginResult result = await this.AuthClient.AuthenticateAsync(
                LiveAuthClient.BuildScopeString(this.scopes),
                silent);

            if (result.Session != null && !LiveAuthClient.AreSessionsSame(this.Session, result.Session))
            {
                this.MergeScopes();
                this.Session = result.Session;
            }
            
            Interlocked.Decrement(ref this.asyncInProgress);

            if (result.Error != null)
            {
                throw result.Error;
            }

            return result;
        }

        private static bool AreSessionsSame(LiveConnectSession session1, LiveConnectSession session2)
        {
            if (session1 != null && session2 != null)
            {
                return
                    session1.AccessToken == session2.AccessToken &&
                    session1.AuthenticationToken == session2.AuthenticationToken;
            }

            return session1 == session2;
        }
        
        private static bool IsValidRedirectDomain(string redirectDomain)
        {
            if (!redirectDomain.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                !redirectDomain.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var redirectUri = new Uri(redirectDomain, UriKind.Absolute);
                return redirectUri.IsWellFormedOriginalString();
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void EnsureSignInScope()
        {
            Debug.Assert(this.scopes != null);

            if (string.IsNullOrEmpty(this.scopes.Find(s => string.CompareOrdinal(s, LiveAuthClient.SignInOfferName) == 0)))
            {
                this.scopes.Insert(0, LiveAuthClient.SignInOfferName);
            }
        }

        private string GetAppPackageSid()
        {
            Uri redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            Debug.Assert(redirectUri != null);

            return redirectUri.AbsoluteUri;
        }

        private void MergeScopes()
        {
            Debug.Assert(this.scopes != null);

            if (this.currentScopes == null)
            {
                this.currentScopes = new List<string>(this.scopes);
                return;
            }

            foreach (string newScope in this.scopes)
            {
                if (!this.currentScopes.Contains(newScope))
                {
                    this.currentScopes.Add(newScope);
                }
            }
        }

        private async void TryRefreshToken(Action<LiveLoginResult> completionCallback)
        {
            LiveLoginResult result = await this.AuthClient.AuthenticateAsync(
                LiveAuthClient.BuildScopeString(this.currentScopes),
                true);

            if (result.Status == LiveConnectSessionStatus.NotConnected &&
                this.currentScopes.Count > 1)
            {
                // The user might have revoked one of the scopes while the app is running. Try getting a token for the remaining scopes
                // by passing in only the "wl.signin" scope. The server should return a token that contains all remaining scopes.
                this.currentScopes = new List<string>(new string[] { LiveAuthClient.SignInOfferName });
                result = await this.AuthClient.AuthenticateAsync(
                    LiveAuthClient.BuildScopeString(this.currentScopes),
                    true);

            }

            if (result.Status == LiveConnectSessionStatus.Unknown)
            {
                // If the auth result indicates that the current account is not connected, we should clear the session
                this.Session = null;
            }
            else if (result.Session != null && !LiveAuthClient.AreSessionsSame(this.Session, result.Session))
            {
                this.Session = result.Session;
            }

            completionCallback(result);
        }

        #endregion
    }
}
