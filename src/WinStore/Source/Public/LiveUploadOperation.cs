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
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Operations;

    /// <summary>
    /// Represents an upload operation.
    /// </summary>
    public class LiveUploadOperation
    {
        private const uint MaxUploadResponseLength = 10240;
        private readonly UploadOperation uploadOperation;

        /// <summary>
        /// Initializes a new instance of LiveUplaodOperation class.
        /// </summary>
        /// <param name="uploadOperaton"></param>
        internal LiveUploadOperation(UploadOperation uploadOperation)
        {
            this.uploadOperation = uploadOperation;
        }

        /// <summary>
        /// Gets the unique ID that represents the upload operation instance.
        /// </summary>
        public Guid Guid
        {
            get { return this.uploadOperation.Guid; }
        }
       
        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <returns></returns>
        public Task<LiveOperationResult> StartAsync()
        {
            return this.ExecuteAsync(true, CancellationToken.None, null);
        }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        public Task<LiveOperationResult> StartAsync(CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler)
        {
            return this.ExecuteAsync(true, cancellationToken, progressHandler);
        }

        /// <summary>
        /// Attaches to the operation.
        /// </summary>
        /// <returns></returns>
        public Task<LiveOperationResult> AttachAsync()
        {
            return this.ExecuteAsync(false, CancellationToken.None, null);
        }

        /// <summary>
        /// Attaches to the operation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        public Task<LiveOperationResult> AttachAsync(CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler)
        {
            return this.ExecuteAsync(false, cancellationToken, progressHandler);
        }

        private async Task<LiveOperationResult> ExecuteAsync(bool start, CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler)
        {
            LiveOperationResult opResult;
            Exception error = null;
            try
            {
                var progressHandlerWrapper = new Progress<UploadOperation>(t =>
                {
                    if (progressHandler != null)
                    {
                        progressHandler.Report(
                            new LiveOperationProgress(
                                (long)t.Progress.BytesSent, //uploading, not downloading
                                (long)t.Progress.TotalBytesToSend //uploading, not downloading
                                ));
                    }
                });

                IAsyncOperationWithProgress<UploadOperation, UploadOperation> asyncOperation = start ? this.uploadOperation.StartAsync() : this.uploadOperation.AttachAsync();
                await asyncOperation.AsTask(cancellationToken, progressHandlerWrapper);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception exp)
            {
                // This might be an server error. We will read the response to determine the error message.
                error = exp;
            }

            IInputStream responseStream = this.uploadOperation.GetResultStreamAt(0);
            if (responseStream == null)
            {
                throw new LiveConnectException(
                    ApiOperation.ApiClientErrorCode,
                    ResourceHelper.GetString("ConnectionError"));
            }
            else
            {
                var reader = new DataReader(responseStream);
                uint length = await reader.LoadAsync(MaxUploadResponseLength);
                opResult = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Upload);

                if (opResult.Error != null)
                {
                    if (opResult.Error is LiveConnectException)
                    {
                        throw opResult.Error;
                    }
                    else if (error != null)
                    {
                        // If the error did not come from the api service,
                        // we'll just return the error thrown by the uploader.
                        throw error;
                    }
                }

                return opResult;
            }
        }
    }
}
