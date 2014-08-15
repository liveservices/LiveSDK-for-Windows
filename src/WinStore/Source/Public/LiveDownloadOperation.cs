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
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage.Streams;

    using Operations;

    /// <summary>
    /// Represents a background download operation.
    /// </summary>
    public class LiveDownloadOperation
    {
        private const uint MaxDownloadResponseLength = 10240;
        private readonly DownloadOperation downloadOperation;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="downloadOperation"></param>
        internal LiveDownloadOperation(DownloadOperation downloadOperation)
        {
            Debug.Assert(downloadOperation != null);

            this.downloadOperation = downloadOperation;
        }

        /// <summary>
        /// Gets the unique ID that represents the download operation instance.
        /// </summary>
        public Guid Guid
        {
            get
            {
                return this.downloadOperation.Guid;
            }
        }

        /// <summary>
        /// Starts the operation
        /// </summary>
        /// <returns></returns>
        public Task<LiveDownloadOperationResult> StartAsync()
        {
            return this.ExecuteAsync(true, CancellationToken.None, null);
        }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        public Task<LiveDownloadOperationResult> StartAsync(CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler = null)
        {
            return this.ExecuteAsync(true, cancellationToken, progressHandler);
        }

        /// <summary>
        /// Attaches to the operation.
        /// </summary>
        /// <returns></returns>
        public Task<LiveDownloadOperationResult> AttachAsync()
        {
            return this.ExecuteAsync(false, CancellationToken.None, null);
        }

        /// <summary>
        /// Attaches to the operation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        public Task<LiveDownloadOperationResult> AttachAsync(CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler = null)
        {
            return this.ExecuteAsync(false, cancellationToken, progressHandler);
        }

        private async Task<LiveDownloadOperationResult> ExecuteAsync(bool start, CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler)
        {
            LiveDownloadOperationResult result = null;
            Exception error = null;
            try
            {
                var progressHandlerWrapper = new Progress<DownloadOperation>(t => {
                    if (progressHandler != null)
                    {
                        progressHandler.Report(
                            new LiveOperationProgress(
                                (long)t.Progress.BytesReceived,
                                (long)t.Progress.TotalBytesToReceive));
                    }
                });

                IAsyncOperationWithProgress<DownloadOperation, DownloadOperation> asyncOperation = start ? this.downloadOperation.StartAsync() : this.downloadOperation.AttachAsync();
                await asyncOperation.AsTask(cancellationToken, progressHandlerWrapper);

                result = this.downloadOperation.ResultFile != null ?
                         new LiveDownloadOperationResult(this.downloadOperation.ResultFile) :
                         new LiveDownloadOperationResult(this.downloadOperation.GetResultStreamAt(0));
                return result;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                error = ex;
            }

            error = await this.ProcessDownloadErrorResponse(error);
            throw error;
        }

        /// <summary>
        /// Process the error response.
        /// </summary>
        private async Task<Exception> ProcessDownloadErrorResponse(Exception exception)
        {
            Exception error;
            try
            {
                IInputStream responseStream = this.downloadOperation.GetResultStreamAt(0);
                if (responseStream == null)
                {
                    error = new LiveConnectException(
                        ApiOperation.ApiServerErrorCode,
                        ResourceHelper.GetString("ConnectionError"));
                }
                else
                {
                    var reader = new DataReader(responseStream);
                    uint length = await reader.LoadAsync(MaxDownloadResponseLength);
                    error = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Download).Error;
                    if (error is FormatException)
                    {
                        // if we cannot understand the error response,
                        // return the exception thrown by the background downloader.
                        error = exception;
                    }
                }
            }
            catch (COMException exp)
            {
                error = exp;
            }
            catch (FileNotFoundException exp)
            {
                error = exp;
            }

            return error;
        }
    }
}
