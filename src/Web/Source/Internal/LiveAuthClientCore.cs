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
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    
    /// <summary>
    /// LiveAuthClientCore class provides the core implementation of authentication/authorization logic
    /// behind the public LiveAuthClient class
    /// </summary>
    internal class LiveAuthClientCore
    {
        private readonly LiveAuthClient publicAuthClient;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly object clientSecrets;
        private readonly IRefreshTokenHandler refreshTokenHandler;
        private IEnumerable<string> initScopes;
        private HttpContextBase webContext;
        private LiveLoginResult loginStatus;
        private RefreshTokenInfo refreshTokenInfo;
        private TaskCompletionSource<LiveLoginResult> currentTask;
        private string appRequestState;
        private string appRequestTs;

        private delegate void CompleteTaskHandler(LiveLoginResult session);

        /// <summary>
        /// Initializes an new instance of the LiveAuthClientCore class.
        /// </summary>
        public LiveAuthClientCore(
            string clientId,
            string clientSecret,
            IRefreshTokenHandler refreshTokenHandler,
            LiveAuthClient authClient)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(clientSecret));
            Debug.Assert(authClient != null);

            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.clientSecrets = clientSecret;
            this.refreshTokenHandler = refreshTokenHandler;
            this.publicAuthClient = authClient;

        }

        /// <summary>
        /// Initializes an new instance of the LiveAuthClientCore class.
        /// </summary>
        public LiveAuthClientCore(
            string clientId,
            IDictionary<int, string> clientSecretMap, 
            IRefreshTokenHandler refreshTokenHandler,
            LiveAuthClient authClient)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(clientSecretMap != null && clientSecretMap.Count > 0);
            Debug.Assert(authClient != null);

            this.clientId = clientId;
            this.clientSecrets = clientSecretMap;
            this.refreshTokenHandler = refreshTokenHandler;
            this.publicAuthClient = authClient;

            // Get latest version
            int largestIndex = clientSecretMap.Keys.First();
            if (clientSecretMap.Count > 1)
            {
                foreach (int index in clientSecretMap.Keys)
                {
                    if (index > largestIndex)
                    {
                        largestIndex = index;
                    }
                }
            }

            this.clientSecret = clientSecretMap[largestIndex];
        }
        
        /// <summary>
        /// Decrypts the user ID from a given authentication token.
        /// </summary>
        public bool GetUserId(string authenticationToken, out string userId, out LiveAuthException error)
        {
            Debug.Assert(!string.IsNullOrEmpty(authenticationToken));

            return LiveAuthWebUtility.ReadUserIdFromAuthenticationToken(
                authenticationToken,
                this.clientSecrets,
                out userId,
                out error);
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
        public Task<LiveLoginResult> InitializeWebSessionAsync(
            string redirectUrl,
            HttpContextBase webContext,
            IEnumerable<string> scopes)
        {
            Debug.Assert(webContext != null);
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));

            this.ValidateConflictAuthTask();

            this.initScopes = scopes;
            this.webContext = webContext;

            // We intentionally move the init logic into a wrapping asynchronous process to work around an issue where
            // invoking Task Async methods will trigger an error to be thrown on an Async Asp.Net page (.aspx) code.
            this.currentTask = new TaskCompletionSource<LiveLoginResult>();
            Task.Factory.StartNew(() =>
            {
                this.InitializeAsync(redirectUrl);
            });

            return this.currentTask.Task;
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
        public Task<LiveLoginResult> ExchangeAuthCodeAsync(string redirectUrl, HttpContextBase webContext)
        {
            Debug.Assert(webContext != null);
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));

            this.ValidateConflictAuthTask();

            this.webContext = webContext;
            var taskSource = new TaskCompletionSource<LiveLoginResult>();
            this.currentTask = taskSource;

            string authorizationCode;
            LiveAuthException error;
            this.LoadSession(out authorizationCode, out error);

            // If this page receives an authorization code, then exchange the code
            // with the auth server to get the tokens.
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // We intentionally move auth actions into a wrapping asynchronous process to work around an issue where
                // invoking Task Async methods will trigger an error to be thrown on an Async Asp.Net page (.aspx) code.
                Task.Factory.StartNew(() =>
                {
                    this.ExchangeCodeForToken(
                        redirectUrl,
                        authorizationCode);
                });
            }
            else if (error != null)
            {
                // We received error from the auth server response.
                if (error.ErrorCode == AuthErrorCodes.AccessDenied)
                {
                    // Access_denied should be treated as NotConnected.
                    LiveLoginResult result = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
                    this.OnAuthTaskCompleted(result);
                }
                else
                {
                    // We received some other error, then throw it.
                    this.OnAuthTaskCompleted(new LiveLoginResult(error));
                }
            }
            else
            {
                // This is exchange auth code only action, but there is neither code nor error return.
                // The app developer may invoke this at wrong location.
                error = new LiveAuthException(AuthErrorCodes.ClientError, ErrorText.AuthServerResponseNotAvailable);
                this.OnAuthTaskCompleted(new LiveLoginResult(error));
            }

            return taskSource.Task;
        }

        private void InitializeAsync(string redirectUrl)
        {
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));

            // Check if we already have one result already.
            if (this.loginStatus != null)
            {
                // We have a result already, then return this one.
                this.OnAuthTaskCompleted(null);
                return;
            }

            string authorizationCode;
            LiveAuthException error;
            this.LoadSession(out authorizationCode, out error);

            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // If this page receives an authorization code, then exchange the code
                // with the auth server to get the tokens.            
                this.ExchangeCodeForToken(
                    redirectUrl,
                    authorizationCode);
            }
            else if (error != null && error.ErrorCode != AuthErrorCodes.AccessDenied)
            {
                // If we got error other than access_denied, we should raise the error
                this.OnAuthTaskCompleted(new LiveLoginResult(error));
            }
            else
            {
                // Otherwise, try refresh token
                this.TryRefreshToken(redirectUrl);
            }
        }

        /// <summary>
        /// Initializes the LiveAuthClient instance. 
        /// This will trigger retrieving token with refresh token process.
        /// </summary>
        public Task<LiveLoginResult> InitializeSessionAsync(
            string redirectUrl, 
            IEnumerable<string> scopes)
        {
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));

            this.ValidateConflictAuthTask();

            this.initScopes = scopes;

            // We intentionally move the init logic into an wrapping asynchrounous process to work around an issue that
            // invoking Task Async methods will trigger an error to be thrown on an Async Asp.Net page (.aspx) code.
            this.currentTask = new TaskCompletionSource<LiveLoginResult>();
            Task.Factory.StartNew(() =>
            {
                this.InitializeAsync(redirectUrl);
            });

            return this.currentTask.Task;
        }

        /// <summary>
        /// Generates a consent URL that includes a set of provided  parameters.
        /// </summary>
        public string GetLoginUrl(IEnumerable<string> scopes, string redirectUrl, DisplayType display, string locale, string state)
        {
            return LiveAuthUtility.BuildAuthorizeUrl(this.clientId, redirectUrl, scopes, ResponseType.Code, display, ThemeType.None, locale, state);
        }

        /// <summary>
        /// Generates a logout URL.
        /// </summary>
        public string GetLogoutUrl(string redirectUrl)
        {
            return LiveAuthUtility.BuildLogoutUrl(this.clientId, redirectUrl);
        }

        /// <summary>
        /// Clear the auth state in the current session
        /// </summary>
        public void ClearSession(HttpContextBase context)
        {
            LiveUtility.ValidateNotNullParameter(context, "context");

            HttpContextUtility.ClearUserSession(context);
            this.loginStatus = null;
            this.publicAuthClient.Session = null;
            this.publicAuthClient.FirePendingPropertyChangedEvents();
        }

        private void ValidateConflictAuthTask()
        {
            // We don't allow auth tasks to be executed concurrently.
            if (this.currentTask != null)
            {
                throw new InvalidOperationException(ErrorText.ExistingAuthTaskRunning);
            }
        }

        private void LoadSession(out string authCode, out LiveAuthException error)
        {
            authCode = null;
            error = null;

            // only load session once.
            if (this.loginStatus == null)
            {
                if (this.webContext != null)
                {
                    // Reads current login status from session cookie.
                    this.loginStatus = HttpContextUtility.GetUserLoginStatus(this.webContext);

                    HttpContextUtility.ReadAuthCodeRequest(
                        webContext, out authCode, out this.appRequestState, out this.appRequestTs, out error);
                    if (this.loginStatus.Status == LiveConnectSessionStatus.Unknown &&
                        error != null && error.ErrorCode == AuthErrorCodes.AccessDenied)
                    {
                        this.loginStatus = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
                    }
                }
                else
                {
                    this.loginStatus = new LiveLoginResult(LiveConnectSessionStatus.Unknown, null);
                }

                this.publicAuthClient.Session = this.loginStatus.Session;
            }
        }

        private void OnAuthTaskCompleted(
            LiveLoginResult loginResult)
        {
            if (loginResult != null)
            {
                if (loginResult.Session != null)
                {
                    LiveAuthException error = this.ValidateSession(loginResult.Session);
                    if (error != null)
                    {
                        loginResult = new LiveLoginResult(error);
                    }
                    else
                    {
                        // We have a new session, update the LiveAuthClient.Session
                        this.loginStatus = loginResult;

                        if (this.refreshTokenHandler != null &&
                            !string.IsNullOrEmpty(loginResult.Session.RefreshToken))
                        {
                            string userId;
                            if (this.GetUserId(loginResult.Session.AuthenticationToken, out userId, out error))
                            {
                                RefreshTokenInfo refreshInfo = new RefreshTokenInfo(loginResult.Session.RefreshToken, userId);
                                Task saveTokenTask = this.refreshTokenHandler.SaveRefreshTokenAsync(refreshInfo);
                                saveTokenTask.ContinueWith((tk) =>
                                {
                                    this.CompleteAuthTask(loginResult);
                                });
                                return;
                            }
                            else
                            {
                                loginResult = new LiveLoginResult(error);
                            }
                        }
                    }
                }
            }
            else
            {
                // We should return the existing status for cases like already initialized or can't refresh ticket.
                loginResult = this.loginStatus;
            }

            this.CompleteAuthTask(loginResult);
        }

        private void CompleteAuthTask(LiveLoginResult loginResult)
        {
            Debug.Assert(loginResult != null);

            loginResult = this.ValidateSessionInitScopes(loginResult);            
            HttpContextUtility.UpdateUserSession(this.webContext, loginResult, this.appRequestTs);            

            if (loginResult.Session != null)
            {
                // Only update Session property if there is a new session.
                this.publicAuthClient.Session = loginResult.Session;
            }

            this.publicAuthClient.FirePendingPropertyChangedEvents();

            TaskCompletionSource<LiveLoginResult> taskSource = this.currentTask;
            if (taskSource != null)
            {
                this.currentTask = null;

                if (loginResult.Error != null)
                {
                    var error = loginResult.Error as LiveAuthException;
                    if (error == null)
                    {
                        error = new LiveAuthException(AuthErrorCodes.ClientError, error.Message, loginResult.Error);
                    }

                    error.State = this.appRequestState;
                    taskSource.SetException(loginResult.Error);
                }
                else
                {
                    loginResult.State = this.appRequestState;
                    taskSource.SetResult(loginResult);
                }
            }
        }

        private LiveLoginResult ValidateSessionInitScopes(LiveLoginResult loginResult)
        {
            if (loginResult.Session != null && this.initScopes != null)
            {
                if (!LiveAuthUtility.IsSubsetOfScopeRange(this.initScopes, loginResult.Session.Scopes))
                {
                    loginResult = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
                }

                this.initScopes = null;
            }

            return loginResult;
        }

        private void ExchangeCodeForToken(string redirectUrl, string authorizationCode)
        {
            Task<LiveLoginResult> task = LiveAuthRequestUtility.ExchangeCodeForTokenAsync(
                     this.clientId, this.clientSecret, redirectUrl, authorizationCode);
            task.ContinueWith((tk) =>
            {
                this.OnAuthTaskCompleted(tk.Result);                
            });
        }

        private void TryRefreshToken(string redirectUrl)
        {
            Debug.Assert(this.loginStatus != null);

            IEnumerable<string> scopes;
            LiveAuthException error;
            bool isTokenRequest = this.CheckRefreshTokenRequest(out scopes, out error);
            if (error != null)
            {
                this.OnAuthTaskCompleted(new LiveLoginResult(error));
                return;
            }

            // Try to refresh a token if 
            // i) there is a token request or
            // ii) we don't have a token or 
            // iii) the current token is expired.    
            LiveLoginResult result = null;
            LiveConnectSession session = this.loginStatus.Session;
            bool hasValidToken = session != null && session.IsValid;
            bool shouldRefresh = (this.refreshTokenHandler != null) && (isTokenRequest || !hasValidToken);

            if (!shouldRefresh)
            {
                this.OnAuthTaskCompleted(null);
                return;
            }

            if (this.initScopes == null)
            {
                // We don't have initScopes, then use the scopes received from Url.
                this.initScopes = scopes;
            }

            this.refreshTokenHandler.RetrieveRefreshTokenAsync().ContinueWith(t =>
            {
                try
                {
                    this.refreshTokenInfo = t.Result;
                    if (this.refreshTokenInfo != null)
                    {
                        string currentUserId = this.publicAuthClient.CurrentUserId;
                        if (currentUserId != null && this.refreshTokenInfo.UserId != currentUserId)
                        {
                            // There is a user Id available in current session. We need to ensure the token provided matches it.
                            result = new LiveLoginResult(new LiveAuthException(
                                AuthErrorCodes.InvalidRequest, ErrorText.RefereshTokenNotMatchUserId));
                        }
                        else
                        {
                            LiveAuthRequestUtility.RefreshTokenAsync(
                               this.clientId,
                               this.clientSecret,
                               redirectUrl,
                               this.refreshTokenInfo.RefreshToken,
                               null/*scopes -  We intentially specify null scopes and validate the initScopes after we have the session 
                                    * result. With this approach, we can return notConnected if initScopes is not satisfied, and surface
                                    * the error if there is one.
                                    */
                               ).ContinueWith((Task<LiveLoginResult> rt) =>
                            {
                                result = rt.Result;
                                this.OnAuthTaskCompleted(result);
                            });
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = new LiveAuthException(AuthErrorCodes.ClientError, ErrorText.RetrieveRefreshTokenError, ex);
                    result = new LiveLoginResult(error);
                }

                this.OnAuthTaskCompleted(result);
            });
        }

        private bool CheckRefreshTokenRequest(out IEnumerable<string> scopes, out LiveAuthException error)
        {
            string clientIdFromRequestUrl;
            error = null;
            bool isTokenRequest = HttpContextUtility.ReadRefreshTokenRequest(this.webContext, out clientIdFromRequestUrl, out scopes);

            if (isTokenRequest)
            {
                if (string.Compare(clientIdFromRequestUrl, this.clientId, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // The request client Id does not match current client Id.
                    error = new LiveAuthException(AuthErrorCodes.ClientError, ErrorText.RefreshRequestClientIdNotMatch);
                }

                if (this.refreshTokenHandler == null)
                {
                    // The web client is requesting requesting refresh token, however, the server has not implemented this logic.
                    error = new LiveAuthException(AuthErrorCodes.ClientError, ErrorText.IRefreshTokenHandlerNotProvided);
                }
            }

            return isTokenRequest;
        }

        /// <summary>
        /// Validate if the user Id from the received session matches the one from the refresh token and current session.
        /// </summary>
        private LiveAuthException ValidateSession(LiveConnectSession session)
        {
            Debug.Assert(session != null);

            string currentUserId = null;
            string userId;
            LiveAuthException error = null;
            LiveConnectSession currentSession = (this.loginStatus == null) ? null : this.loginStatus.Session;

            // Read current session user Id, if available.
            if (currentSession != null)
            {
                LiveAuthException currentSessionError;
                LiveAuthWebUtility.ReadUserIdFromAuthenticationToken(
                    currentSession.AuthenticationToken,
                    this.clientSecrets,
                    out currentUserId,
                    out currentSessionError);
            }

            // Read user Id from the new session received from the auth server.
            LiveAuthWebUtility.ReadUserIdFromAuthenticationToken(session.AuthenticationToken, this.clientSecrets, out userId, out error);

            if (error == null)
            {
                if (!string.IsNullOrEmpty(currentUserId) &&
                    string.Compare(userId, currentUserId, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // The user Id should match current session user Id
                    error = new LiveAuthException(AuthErrorCodes.InvalidRequest, ErrorText.NewSessionDoesNotMatchCurrentUserId);
                }
                else if (this.refreshTokenInfo != null &&
                    string.Compare(userId, this.refreshTokenInfo.UserId, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // The user Id should match the uesr Id from the one in the refresh token if available.
                    error = new LiveAuthException(AuthErrorCodes.InvalidRequest, ErrorText.RefereshTokenNotMatchUserId);
                }
            }

            return error;
        }
    }
}
