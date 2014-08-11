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
    using System.IO;
    using System.Net;
    using System.Text;

    using Microsoft.Live.Serialization;

    /// <summary>
    /// Represents a single operation that makes a web request to the API service.
    /// </summary>
    internal class ApiOperation : WebOperation
    {
        #region Class member variables

        internal const string ContentTypeJson = @"application/json;charset=UTF-8";
        internal const string AuthorizationHeader = "Authorization";
        internal const string LibraryHeader = "X-HTTP-Live-Library";
        internal const string ApiError = "error";
        internal const string ApiErrorCode = "code";
        internal const string ApiErrorMessage = "message";
        internal const string ApiClientErrorCode = "client_error";
        internal const string ApiServerErrorCode = "server_error";
        internal const string MoveRequestBodyTemplate = @"{{ ""destination"" : ""{0}"" }}";

        #endregion

        #region Instance member variables

        private bool refreshed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new ApiOperation object.
        /// </summary>
        public ApiOperation(
            LiveConnectClient client, 
            Uri url, 
            ApiMethod method, 
            string body, 
            SynchronizationContextWrapper syncContext)
            : base(url, body, syncContext)
        {
            this.Method = method;
            this.LiveClient = client;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the LiveConnectClient object.
        /// </summary>
        public LiveConnectClient LiveClient { get; private set; }

        /// <summary>
        /// Gets the API method this operation represents.
        /// </summary>
        public ApiMethod Method { get; private set; }

        /// <summary>
        /// Gets and sets the operation completed callback delegate.
        /// </summary>
        public Action<LiveOperationResult> OperationCompletedCallback { get; set; }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Serializes the request body.
        /// </summary>
        internal static string SerializePostBody(IDictionary<string, object> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var jw = new JsonWriter(sw);
                jw.WriteValue(body);
                sw.Flush();
            }

            return sb.ToString();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses a string response body and creates a LiveOperationResult object from it.
        /// </summary>
        internal static LiveOperationResult CreateOperationResultFrom(string responseBody, ApiMethod method)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                if (method != ApiMethod.Delete)
                {
                    var error = new LiveConnectException(
                        ApiClientErrorCode, 
                        ResourceHelper.GetString("NoResponseData"));
                    return new LiveOperationResult(error, false);
                }

                return new LiveOperationResult(null, responseBody);
            }

            return LiveOperationResult.FromResponse(responseBody);
        }

        /// <summary>
        /// Parses the WebResponse's body and creates a LiveOperationResult object from it.
        /// </summary>
        protected LiveOperationResult CreateOperationResultFrom(WebResponse response)
        {
            LiveOperationResult opResult;

            bool nullResponse = (response == null);
            try
            {
                Stream responseStream = (!nullResponse) ? response.GetResponseStream() : null;
                if (nullResponse || responseStream == null)
                {
                    var error = new LiveConnectException(
                        ApiOperation.ApiClientErrorCode, 
                        ResourceHelper.GetString("ConnectionError"));

                    opResult = new LiveOperationResult(error, false);
                }
                else
                {
                    using (var sr = new StreamReader(responseStream))
                    {
                        string rawResult = sr.ReadToEnd();
                        opResult = CreateOperationResultFrom(rawResult, this.Method);
                    }
                }
            }
            finally
            {
                if (!nullResponse)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            return opResult;
        }

        /// <summary>
        /// Overwrites the base OnExecute to refresh access token if neccessary.
        /// </summary>
        protected override void OnExecute()
        {
            if (this.PrepareRequest())
            {
                try
                {
                    this.Request.BeginGetResponse(this.OnGetResponseCompleted, null);
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        this.OnWebResponseReceived(exception.Response);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the operation is cancelled.
        /// </summary>
        protected override void OnCancel()
        {
            this.OnOperationCompleted(new LiveOperationResult(null, true));
        }

        /// <summary>
        /// Calls the OperationCompletedCallback delegate.
        /// This method is called when the ApiOperation is completed.
        /// </summary>
        protected void OnOperationCompleted(LiveOperationResult opResult)
        {
            Action<LiveOperationResult> callback = this.OperationCompletedCallback;
            if (callback != null)
            {
                callback(opResult);
            }
        }

        /// <summary>
        /// Called when the operation has a WebResponse from the server to handle.
        /// </summary>
        protected override void OnWebResponseReceived(WebResponse response)
        {
            this.OnOperationCompleted(this.CreateOperationResultFrom(response));
        }

        /// <summary>
        /// Prepares the web request. Sets up the correct method, headers, etc.
        /// </summary>
        protected bool PrepareRequest()
        {
            if (!this.RefreshTokenIfNeeded())
            {
                string httpMethod;

                switch (this.Method)
                {
                    case ApiMethod.Upload:
                        httpMethod = HttpMethods.Put;
                        break;

                    case ApiMethod.Download:
                        httpMethod = HttpMethods.Get;
                        break;

                    default:
                        httpMethod = this.Method.ToString().ToUpperInvariant();
                        break;
                }

                this.Request = WebRequestFactory.Current.CreateWebRequest(this.Url, httpMethod);
                if (this.LiveClient.Session != null)
                {
                    this.Request.Headers[ApiOperation.AuthorizationHeader] =
                        AuthConstants.BearerTokenType + " " + this.LiveClient.Session.AccessToken;
                }
                this.Request.Headers[ApiOperation.LibraryHeader] = Platform.GetLibraryHeaderValue();

                if (!string.IsNullOrEmpty(this.Body))
                {
                    this.Request.ContentType = ApiOperation.ContentTypeJson;
                }
                 
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the access token is still valid.  If not, refreshes the token.
        /// </summary>
        protected bool RefreshTokenIfNeeded()
        {
            bool needsRefresh = false;
            var session = this.LiveClient.Session;
            LiveAuthClient authClient = (session != null) ? session.AuthClient : null;
            if (!this.refreshed && authClient != null)
            {
                this.refreshed = true;

                needsRefresh = authClient.RefreshToken(this.OnRefreshTokenOperationCompleted);
            }

            return needsRefresh;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the refresh token completed event.  Restarts the operation.
        /// </summary>
        private void OnRefreshTokenOperationCompleted(LiveLoginResult result)
        {
            switch (result.Status)
            {
                case LiveConnectSessionStatus.Connected:
                    this.LiveClient.Session = result.Session;
                    break;
                case LiveConnectSessionStatus.Unknown:
                    // Once we know the user is unknown, we clear the session and fail the operation. 
                    // On Windows Blue, the user may disconnect the Microsoft account. 
                    // We ensure we are not allowing app to continue to access user's data after the user disconnects the account.
                    this.LiveClient.Session = null;
                    var error = new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
                    this.OnOperationCompleted(new LiveOperationResult(error, false));
                    return;
            }

            // We will attempt to perform the operation even if refresh fails.
            this.InternalExecute();
        }

        #endregion
    }
}
