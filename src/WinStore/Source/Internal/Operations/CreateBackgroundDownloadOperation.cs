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
    /// This class implements the download operation on Windows 8 using the 
    /// background downloader.
    /// </summary>
    internal class CreateBackgroundDownloadOperation : ApiOperation
    {
        private DownloadOperation downloadOp;
        private TaskCompletionSource<LiveDownloadOperation> taskCompletionSource;

        /// <summary>
        /// Creates a new TailoredDownloadOperation instance.
        /// </summary>
        public CreateBackgroundDownloadOperation(
            LiveConnectClient client, 
            Uri url, 
            IStorageFile outputFile)
            : base(client, url, ApiMethod.Download, null, null)
        {
            this.OutputFile = outputFile;
        }

        /// <summary>
        /// Gets the IStorageFile object that the downloaded content is to be stored in.
        /// </summary>
        public IStorageFile OutputFile { get; private set; }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<LiveDownloadOperation> ExecuteAsync()
        {
            this.taskCompletionSource = new TaskCompletionSource<LiveDownloadOperation>();
            this.Execute();
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Overrides the base execute logic to use the background downloader to download the file.
        /// </summary>
        protected async override void OnExecute()
        {
            var loginResult = await this.LiveClient.Session.AuthClient.GetLoginStatusAsync();
            if (loginResult.Error != null)
            {
                this.taskCompletionSource.SetException(loginResult.Error);
                return;
            }
            else
            {
                var downloader = new BackgroundDownloader();
                downloader.SetRequestHeader(
                        ApiOperation.AuthorizationHeader,
                        AuthConstants.BearerTokenType + " " + loginResult.Session.AccessToken);
                downloader.SetRequestHeader(ApiOperation.LibraryHeader, Platform.GetLibraryHeaderValue());

                downloader.Group = LiveConnectClient.LiveSDKDownloadGroup;
                this.downloadOp = downloader.CreateDownload(this.Url, this.OutputFile);
                this.taskCompletionSource.SetResult(new LiveDownloadOperation(downloadOp));
            }
        }
    }
}
