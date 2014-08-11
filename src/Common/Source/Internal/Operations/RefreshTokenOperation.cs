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

namespace Microsoft.Live.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Live.Serialization;

    /// <summary>
    /// Make a call to the oauth token endpoint using a refresh token and parse
    /// the response to get the new access token.
    /// </summary>
    internal class RefreshTokenOperation : WebOperation
    {
        #region Class member variables

        private const string RefreshTokenPostBodyTemplate = AuthConstants.ClientId + "={0}&" +
                                                            AuthConstants.RefreshToken + "={1}&" +
                                                            AuthConstants.Scope + "={2}&" +
                                                            AuthConstants.GrantType + "=" + AuthConstants.RefreshToken;

        private const string AuthCodePostBodyTemplate = AuthConstants.ClientId + "={0}&" +
                                                        AuthConstants.Code + "={1}&" +
                                                        AuthConstants.Callback + "={2}&" +
                                                        AuthConstants.GrantType + "=" + AuthConstants.AuthorizationCode;

        private const string ContentTypeFormEncoded = @"application/x-www-form-urlencoded";

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new RefreshTokenOperation object.
        /// </summary>
        /// <remarks>This constructor is used when exchanging the refresh token for the access token.</remarks>
        public RefreshTokenOperation(
            LiveAuthClient authClient, 
            string clientId, 
            string refreshToken, 
            IEnumerable<string> scopes,
            SynchronizationContextWrapper syncContext)
            : base(new Uri(authClient.ConsentEndpoint + LiveAuthClient.TokenEndpoint), 
                   null,   
                   syncContext)
        {
            Debug.Assert(authClient != null, "authClient must not be null.");
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(refreshToken));

            this.AuthClient = authClient;

            this.Body = String.Format(
                RefreshTokenOperation.RefreshTokenPostBodyTemplate,
                HttpUtility.UrlEncode(clientId),
                refreshToken,
                HttpUtility.UrlEncode(LiveAuthClient.BuildScopeString(scopes)));
        }

        /// <summary>
        /// Create a new RefreshTokenOperation object.
        /// </summary>
        /// <remarks>This constructor is used when exchanging the verification code for the access token.</remarks>
        public RefreshTokenOperation(
            LiveAuthClient authClient,
            string clientId,
            string verificationCode,
            string redirectUri,
            SynchronizationContextWrapper syncContext)
            : base(new Uri(authClient.ConsentEndpoint + LiveAuthClient.TokenEndpoint), 
                   null, 
                   syncContext)
        {
            Debug.Assert(authClient != null, "authClient must not be null.");
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(verificationCode));

            this.AuthClient = authClient;

            this.Body = String.Format(
                        RefreshTokenOperation.AuthCodePostBodyTemplate,
                        HttpUtility.UrlEncode(clientId),
                        HttpUtility.UrlEncode(verificationCode),
                        HttpUtility.UrlEncode(redirectUri));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the LiveAuthClient object.
        /// </summary>
        public LiveAuthClient AuthClient { get; private set; }

        /// <summary>
        /// Gets and sets the operation completed callback delegate.
        /// </summary>
        public Action<LiveLoginResult> OperationCompletedCallback { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// We do not support cancel for refresh token.
        /// </summary>
        public override void Cancel()
        {
            // no-op
            // We don't support cancel on auth operations.
        }

        /// <summary>
        /// Uses the Task&lt;T&gt; pattern to return a LiveLoginResult.
        /// NOTE: This method will overwrite and uses its own OperationCompletedCallback.
        /// </summary>
        public Task<LiveLoginResult> ExecuteAsync()
        {
            var tcs = new TaskCompletionSource<LiveLoginResult>();
            this.OperationCompletedCallback = (LiveLoginResult result) =>
            {
                if (result.Error != null)
                {
                    tcs.TrySetException(result.Error);
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            };

            this.Execute();

            return tcs.Task;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// We do not support cancel for refresh token.
        /// </summary>
        protected override void OnCancel()
        {
            // No-op
        }

        /// <summary>
        /// Overrides the base OnExecute method to make a POST request to the oauth token endpoint.
        /// </summary>
        protected override void OnExecute()
        {
            this.Request = WebRequestFactory.Current.CreateWebRequest(this.Url, "POST");
            this.Request.ContentType = ContentTypeFormEncoded;

            this.Request.BeginGetRequestStream(this.OnGetRequestStreamCompleted, null);
        }

        /// <summary>
        /// Raises the OperationCompletedCallback.
        /// </summary>
        protected void OnOperationCompleted(LiveLoginResult opResult)
        {
            Action<LiveLoginResult> callback = this.OperationCompletedCallback;
            if (callback != null)
            {
                callback(opResult);
            }
        }

        /// <summary>
        /// Process the web response from the server.
        /// </summary>
        protected override void OnWebResponseReceived(WebResponse response)
        {
            LiveLoginResult result;

            bool nullResponse = (response == null);
            try
            {
                Stream responseStream = (!nullResponse) ? response.GetResponseStream() : null;
                if (nullResponse || responseStream == null)
                {
                    result = new LiveLoginResult(
                        new LiveAuthException(AuthErrorCodes.ClientError, ResourceHelper.GetString("ConnectionError")));
                }
                else
                {
                    result = this.GenerateLoginResultFrom(responseStream);
                }
            }
            finally
            {
                if (!nullResponse)
                {
                    response.Dispose();
                }
            }

            this.OnOperationCompleted(result);
        }

        #endregion

        #region Private methods

        private LiveLoginResult GenerateLoginResultFrom(Stream responseStream)
        {
            IDictionary<string, object> jsonObj;
            try
            {
                using (var reader = new StreamReader(responseStream))
                {
                    var jsReader = new JsonReader(reader.ReadToEnd());
                    jsonObj = jsReader.ReadValue() as IDictionary<string, object>;
                }
            }
            catch (FormatException fe)
            {
                return new LiveLoginResult(
                    new LiveAuthException(AuthErrorCodes.ServerError, fe.Message));
            }

            if (jsonObj == null)
            {
                return new LiveLoginResult(
                        new LiveAuthException(AuthErrorCodes.ServerError, ResourceHelper.GetString("ServerError")));
            }

            if (jsonObj.ContainsKey(AuthConstants.Error))
            {
                var errorCode = jsonObj[AuthConstants.Error] as string;
                if (errorCode.Equals(AuthErrorCodes.InvalidGrant, StringComparison.Ordinal))
                {
                    return new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
                }

                string errorDescription = string.Empty;
                if (jsonObj.ContainsKey(AuthConstants.ErrorDescription))
                {
                    errorDescription = jsonObj[AuthConstants.ErrorDescription] as string;
                }

                return new LiveLoginResult(new LiveAuthException(errorCode, errorDescription));
            }

            LiveConnectSession newSession = LiveAuthClient.CreateSession(this.AuthClient, jsonObj);
            return new LiveLoginResult(LiveConnectSessionStatus.Connected, newSession);
        }

        #endregion
    }
}
