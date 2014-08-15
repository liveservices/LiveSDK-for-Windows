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
