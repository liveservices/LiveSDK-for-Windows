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

namespace Microsoft.Live.Operations
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;

    /// <summary>
    /// This class implements the upload operation on Windows 8 using the background uploader.
    /// </summary>
    internal class CreateBackgroundUploadOperation : ApiOperation
    {
        private TaskCompletionSource<LiveUploadOperation> taskCompletionSource;

        /// <summary>
        /// This class implements the upload operation on Windows 8 using the background uploader.
        /// </summary>
        /// <remarks>This constructor is used when uploading a file from the file system.</remarks>
        public CreateBackgroundUploadOperation(
            LiveConnectClient client, 
            Uri url, 
            string fileName,
            IStorageFile inputFile,
            OverwriteOption option)
            : base(client, url, ApiMethod.Upload, null, null)
        {
            Debug.Assert(inputFile != null, "inputFile is null.");

            this.InputFile = inputFile;
            this.FileName = fileName;
            this.OverwriteOption = option;
        }

        /// <summary>
        /// This class implements the upload operation on Windows 8 using the background uploader.
        /// </summary>
        /// <remarks>This constructor is used when uploading a stream created by the application.</remarks>
        public CreateBackgroundUploadOperation(
            LiveConnectClient client,
            Uri url,
            string fileName,
            IInputStream inputStream,
            OverwriteOption option)
            : base(client, url, ApiMethod.Upload, null, null)
        {
            Debug.Assert(inputStream != null, "inputStream is null.");

            this.InputStream = inputStream;
            this.FileName = fileName;
            this.OverwriteOption = option;
        }

        public string FileName { get; private set; }

        public OverwriteOption OverwriteOption { get; private set; }

        public IStorageFile InputFile { get; private set; }

        public IInputStream InputStream { get; private set; }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<LiveUploadOperation> ExecuteAsync()
        {
            this.taskCompletionSource = new TaskCompletionSource<LiveUploadOperation>();
            this.Execute();
            return taskCompletionSource.Task;
        }

        protected override void OnExecute()
        {
            var getUploadLinkOp = new GetUploadLinkOperation(
                    this.LiveClient,
                    this.Url,
                    this.FileName,
                    this.OverwriteOption,
                    null);

            getUploadLinkOp.OperationCompletedCallback = this.OnGetUploadLinkCompleted;
            getUploadLinkOp.Execute();
        }

        private async void OnGetUploadLinkCompleted(LiveOperationResult result)
        {
            if (result.Error != null)
            {
                this.taskCompletionSource.SetException(result.Error);
                return;
            }

            var uploadUrl = new Uri(result.RawResult, UriKind.Absolute);

            // NOTE: the GetUploadLinkOperation will return a uri with the overwite, suppress_response_codes,
            // and suppress_redirect query parameters set.
            Debug.Assert(uploadUrl.Query != null);
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.Overwrite));
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.SuppressRedirects));
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.SuppressResponseCodes));

            var uploader = new BackgroundUploader();
            uploader.Group = LiveConnectClient.LiveSDKUploadGroup;
            if (this.LiveClient.Session != null)
            {
                uploader.SetRequestHeader(
                    ApiOperation.AuthorizationHeader,
                    AuthConstants.BearerTokenType + " " + this.LiveClient.Session.AccessToken);
            }
            uploader.SetRequestHeader(ApiOperation.LibraryHeader, Platform.GetLibraryHeaderValue());
            uploader.Method = HttpMethods.Put;

            UploadOperation uploadOperation;
            if (this.InputStream != null)
            {
                uploadOperation = await uploader.CreateUploadFromStreamAsync(uploadUrl, this.InputStream);
            }
            else
            {
                uploadOperation = uploader.CreateUpload(uploadUrl, this.InputFile);
            }

            this.taskCompletionSource.SetResult(new LiveUploadOperation(uploadOperation));
        }
    }
}
