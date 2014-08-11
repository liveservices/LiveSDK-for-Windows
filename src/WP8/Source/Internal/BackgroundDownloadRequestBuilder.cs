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

    using Microsoft.Live.Operations;
    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// Class that builds a BackgroundTransferRequest for downloads.
    /// </summary>
    internal class BackgroundDownloadRequestBuilder
    {
        #region Fields

        private Uri requestUri;
        private string accessToken;
        private Uri downloadLocationOnDevice;
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

            var request = new BackgroundTransferRequest(this.requestUri, this.downloadLocationOnDevice);

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
