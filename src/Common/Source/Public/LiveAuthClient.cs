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
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Microsoft.Live.Operations;

    /// <summary>
    /// This is the class that applications use to authenticate the user and obtain access token after user
    /// grants consent to the application.
    /// </summary>
    // TODO: consider making this class a helper class. There is really not much that can be shared between
    //       Win8 & WP for this class anymore.
    public sealed partial class LiveAuthClient : INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// Defines local storage keys.
        /// </summary>
        internal static class StorageConstants
        {
            public const string RefreshToken = "Microsoft.Live.LiveAuthClient.RefreshToken";
            public const string SigningOut = "Microsoft.Live.LiveAuthClient.SigningOut";
        }

        /// <summary>
        /// Default redirect url for desktop apps.
        /// </summary>
        internal static readonly string DefaultRedirectPath = "/oauth20_desktop.srf";

        /// <summary>
        /// Default consent endpoint.
        /// </summary>
        internal static readonly string DefaultConsentEndpoint = "https://login.live.com";

        /// <summary>
        /// Authorize endpoint.
        /// </summary>
        internal static readonly string AuthorizeEndpoint = "/oauth20_authorize.srf";

        /// <summary>
        /// Token endpoint.
        /// </summary>
        internal static readonly string TokenEndpoint = "/oauth20_token.srf";

        /// <summary>
        /// Logout endpoint.
        /// </summary>
        internal static readonly string LogoutUrl = "https://login.live.com/oauth20_logout.srf";

        #endregion

        #region Private Member Fields

        private static readonly char[] ScopeSeparators = new char[] { ' ', ',' };
        private const string AuthorizeUrlTemplate = @"{0}{1}?" + 
                                                    AuthConstants.ClientId + "={2}&" + 
                                                    AuthConstants.Callback + "={3}&" + 
                                                    AuthConstants.Scope + "={4}&" + 
                                                    AuthConstants.ResponseType + "={5}&" +
                                                    "locale={6}&display={7}{8}";

        private string clientId;
        private string redirectUri;
        private LiveConnectSession session;
        private List<string> scopes;

        /// <summary>
        /// Used for locking Initialize and Login operations.
        /// </summary>
        private int asyncInProgress;

        private SynchronizationContextWrapper syncContext;

        #endregion

        #region Properties

#if DEBUG
        /// <summary>
        /// Allows the application to override the default consent endpoint.
        /// </summary>
        public static string AuthEndpointOverride { get; set; }

        /// <summary>
        /// Allows the application to override the default logout endpoint.
        /// </summary>
        public static string LogoutEndpointOverride { get; set; }
#endif

        /// <summary>
        /// Gets the current session object.
        /// </summary>
        public LiveConnectSession Session
        {
            get
            {
                return this.session;
            }

            internal set
            {
                this.session = value;
                this.NotifyPropertyChanged("Session");
            }
        }

        /// <summary>
        /// Gets and sets the response type used for the consent request.
        /// </summary>
        internal ResponseType ResponseType { get; set; }

        /// <summary>
        /// Gets and sets the display type used for the consent request.
        /// </summary>
        internal DisplayType Display { get; set; }

        /// <summary>
        /// Gets the root consent endpoint.
        /// </summary>
        internal string ConsentEndpoint { get; private set; }

        /// <summary>
        /// Gets and sets the IAuthClient instance.
        /// </summary>
        internal IAuthClient AuthClient { get; set; }

        /// <summary>
        /// Gets the redirect url.
        /// </summary>
        internal string RedirectUrl
        {
            get
            {
                return this.redirectUri;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// An event that fires when the Session property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private & internal Methods

        /// <summary>
        /// Constructs the consent url.
        /// </summary>
        internal string BuildLoginUrl(string scopes, bool silent)
        {
            string consentUrl = String.Format(
                LiveAuthClient.AuthorizeUrlTemplate,
                this.ConsentEndpoint,
                AuthorizeEndpoint,
                HttpUtility.UrlEncode(this.clientId),
                HttpUtility.UrlEncode(this.redirectUri),
                HttpUtility.UrlEncode(scopes),
                HttpUtility.UrlEncode(this.ResponseType.ToString().ToLowerInvariant()),
                HttpUtility.UrlEncode(Platform.GetCurrentUICultureString()),
                silent ? "none" : HttpUtility.UrlEncode(this.Display.ToString().ToLowerInvariant()),
                silent ? string.Empty : "&theme=" + HttpUtility.UrlEncode(this.Theme.ToString().ToLowerInvariant()));

            return consentUrl;
        }

        /// <summary>
        /// Converts a list of offers into one single offer string with space as separator.
        /// </summary>
        internal static string BuildScopeString(IEnumerable<string> scopes)
        {
            var sb = new StringBuilder();
            if (scopes != null)
            {
                foreach (string s in scopes)
                {
                    sb.Append(s).Append(LiveAuthClient.ScopeSeparators[0]);
                }
            }

            return sb.ToString().TrimEnd(LiveAuthClient.ScopeSeparators);
        }

        /// <summary>
        /// Calculates when the access token will be expired.
        /// </summary>
        internal static DateTimeOffset CalculateExpiration(string expiresIn)
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow;
            long seconds;
            if (long.TryParse(expiresIn, out seconds))
            {
                expires = expires.AddSeconds(seconds);
            }

            return expires;
        }

        /// <summary>
        /// Converts a single offer string into a list of offers.
        /// </summary>
        internal static IEnumerable<string> ParseScopeString(string scopesString)
        {
            return new List<string>(
                scopesString.Split(LiveAuthClient.ScopeSeparators, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Creates a error LoginResult object.
        /// </summary>
        private static LiveLoginResult GetErrorResult(IDictionary<string, object> result)
        {
            Debug.Assert(result.ContainsKey(AuthConstants.Error));

            var errorCode = result[AuthConstants.Error] as string;
            if (errorCode.Equals(AuthErrorCodes.AccessDenied, StringComparison.Ordinal))
            {
                return new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
            }

            if (errorCode.Equals(AuthErrorCodes.UnknownUser, StringComparison.Ordinal))
            {
                return new LiveLoginResult(LiveConnectSessionStatus.Unknown, null);
            }

            string errorDescription = string.Empty;

            if (result.ContainsKey(AuthConstants.ErrorDescription))
            {
                errorDescription = result[AuthConstants.ErrorDescription] as string;
            }

            return new LiveLoginResult(new LiveAuthException(errorCode, errorDescription));
        }

        /// <summary>
        /// Initializes the member variables.
        /// </summary>
        private void InitializeMembers(string clientId, string redirectUri)
        {
            this.clientId = clientId;

#if DEBUG
            this.ConsentEndpoint =
                string.IsNullOrEmpty(LiveAuthClient.AuthEndpointOverride)
                ? LiveAuthClient.DefaultConsentEndpoint
                : LiveAuthClient.AuthEndpointOverride.TrimEnd('/');
#else
            this.ConsentEndpoint = LiveAuthClient.DefaultConsentEndpoint;
#endif
            if (!string.IsNullOrEmpty(redirectUri))
            {
                this.redirectUri = redirectUri;
            }
            else
            {
                this.redirectUri = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    new Uri(this.ConsentEndpoint).GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped),
                    LiveAuthClient.DefaultRedirectPath);
            }

            this.ResponseType = Platform.GetResponseType();

            this.Display = Platform.GetDisplayType();
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                Debug.Assert(this.syncContext != null);

                this.syncContext.Post(() => handler(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        /// <summary>
        /// Parses the query string into key-value pairs.
        /// </summary>
        private static IDictionary<string, object> ParseQueryString(string query)
        {
            var values = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(query))
            {
                query = query.TrimStart(new char[] { '?', '#' });
                if (!String.IsNullOrEmpty(query))
                {
                    string[] parameters = query.Split(new char[] { '&' });
                    foreach (string parameter in parameters)
                    {
                        string[] pair = parameter.Split(new char[] { '=' });
                        if (pair.Length == 2)
                        {
                            values.Add(pair[0], pair[1]);
                        }
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Parse the response data.
        /// </summary>
        private LiveLoginResult ParseResponseFragment(string fragment)
        {
            Debug.Assert(!string.IsNullOrEmpty(fragment));

            LiveLoginResult loginResult = null;
            IDictionary<string, object> result = LiveAuthClient.ParseQueryString(fragment);
            if (result.ContainsKey(AuthConstants.AccessToken))
            {
                LiveConnectSession sessionData = CreateSession(this, result);

                loginResult = new LiveLoginResult(LiveConnectSessionStatus.Connected, sessionData);
            }
            else if (result.ContainsKey(AuthConstants.Error))
            {
                loginResult = GetErrorResult(result);
            }

            return loginResult;
        }

        /// <summary>
        /// Ensures that only one async operation is active at any time.
        /// </summary>
        private void PrepareForAsync()
        {
            Debug.Assert(
                this.asyncInProgress == 0 || this.asyncInProgress == 1, 
                "Unexpected value for 'asyncInProgress' field.");

            if (this.asyncInProgress > 0)
            {
                throw new LiveAuthException(
                    AuthErrorCodes.ClientError, 
                    ResourceHelper.GetString("AsyncOperationInProgress"));
            }

            Interlocked.Increment(ref this.asyncInProgress);

            this.syncContext = SynchronizationContextWrapper.Current;
        }

        /// <summary>
        /// Processes authentication result from the server.
        /// Method could return synchronously or asynchronously.
        /// </summary>
        private void ProcessAuthResponse(string responseData, Action<LiveLoginResult> callback)
        {
            if (string.IsNullOrEmpty(responseData))
            {
                // non-connected user scenario. return status unknown.
                callback(new LiveLoginResult(LiveConnectSessionStatus.Unknown, null));
                return;
            }

            Uri responseUrl;
            try
            {
                responseUrl = new Uri(responseData, UriKind.Absolute);
            }
            catch (FormatException)
            {
                callback(new LiveLoginResult(
                    new LiveAuthException(AuthErrorCodes.ServerError, ResourceHelper.GetString("ServerError"))));
                return;
            }

            if (!string.IsNullOrEmpty(responseUrl.Fragment))
            {
                callback(this.ParseResponseFragment(responseUrl.Fragment));
                return;
            }

            if (!string.IsNullOrEmpty(responseUrl.Query))
            {
                IDictionary<string, object> parameters = LiveAuthClient.ParseQueryString(responseUrl.Query);
                if (parameters.ContainsKey(AuthConstants.Code))
                {
                    var authCode = parameters[AuthConstants.Code] as string;
                    if (!string.IsNullOrEmpty(authCode))
                    {
                        var refreshOp = new RefreshTokenOperation(
                            this, 
                            this.clientId,
                            authCode,
                            this.redirectUri,
                            this.syncContext);

                        refreshOp.OperationCompletedCallback = callback;
                        refreshOp.Execute();

                        return;
                    }
                }
                else if (parameters.ContainsKey(AuthConstants.Error))
                {
                    callback(GetErrorResult(parameters));
                    return;
                }
            }

            callback(
                new LiveLoginResult(
                    new LiveAuthException(AuthErrorCodes.ServerError, ResourceHelper.GetString("ServerError"))));
        }

        #endregion
    }
}
