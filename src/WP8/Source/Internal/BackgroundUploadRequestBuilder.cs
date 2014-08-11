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

namespace Microsoft.Live.Phone
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using Microsoft.Live.Operations;
    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// Class that builds a BackgroundTransferRequest for uploads.
    /// </summary>
    internal class BackgroundUploadRequestBuilder
    {
        #region Fields

        private const char Ampersand = '&';

        private Uri requestUri;
        private string accessToken;
        private Uri downloadLocationOnDevice;
        private Uri uploadLocationOnDevice;
        private TransferPreferences transferPreferences;

        #endregion

        #region Properties

        public Uri RequestUri
        {
            set
            {
                Debug.Assert(value != null);
                this.requestUri = value;
            }
        }

        public string AccessToken
        {
            set
            {
                Debug.Assert(value != null);
                this.accessToken = value;
            }
        }

        public Uri DownloadLocationOnDevice
        {
            set
            {
                Debug.Assert(value != null);
                this.downloadLocationOnDevice = value;
            }
        }

        public Uri UploadLocationOnDevice
        {
            set
            {
                Debug.Assert(value != null);
                this.uploadLocationOnDevice = value;
            }
        }

        public TransferPreferences TransferPreferences
        {
            set
            {
                this.transferPreferences = value;
            }
        }

        #endregion

        #region Methods

        public BackgroundTransferRequest Build()
        {
            Debug.Assert(this.requestUri != null);
            Debug.Assert(this.accessToken != null);
            Debug.Assert(this.downloadLocationOnDevice != null);
            Debug.Assert(this.uploadLocationOnDevice != null);

            var builder = new UriBuilder(this.requestUri);

            // NOTE: the GetUploadLinkOperation will return a uri with the overwite, suppress_response_codes,
            // and suppress_redirect query parameters set.
            Debug.Assert(builder.Query != null);
            Debug.Assert(builder.Query.Contains(QueryParameters.Overwrite));
            Debug.Assert(builder.Query.Contains(QueryParameters.SuppressRedirects));
            Debug.Assert(builder.Query.Contains(QueryParameters.SuppressResponseCodes));

            var queryParams = new StringBuilder();
            if (builder.Query.Length > 1)
            {
                queryParams.Append(builder.Query.Substring(1))
                           .Append(Ampersand);
            }

            // BackgroundTransferRequests can only be GET or POST, but the service expects
            // a multipart/form-data body from POST. We can change the POST request to a PUT
            // by using the method query parameter.
            queryParams.AppendQueryParam(QueryParameters.Method, HttpMethods.Put);

            builder.Query = queryParams.ToString();

            var request = new BackgroundTransferRequest(builder.Uri, this.downloadLocationOnDevice);

            try
            {
                request.UploadLocation = this.uploadLocationOnDevice;
            }
            catch (InvalidOperationException e)
            {
                // Thrown if the request's properties are modified after it has been submitted or disposed.
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode,
                                               "BackgroundTransferRequest threw an exception.",
                                               e);
            }

            request.Method = HttpMethods.Post;
            request.Headers.Add(
                ApiOperation.AuthorizationHeader, 
                AuthConstants.BearerTokenType + " " + this.accessToken);
            request.Headers.Add(ApiOperation.LibraryHeader, Platform.GetLibraryHeaderValue());

            request.Tag = BackgroundTransferHelper.Tag;

            request.TransferPreferences = this.transferPreferences;

            return request;
        }

        #endregion
    }
}
