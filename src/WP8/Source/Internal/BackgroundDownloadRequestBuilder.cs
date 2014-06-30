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
