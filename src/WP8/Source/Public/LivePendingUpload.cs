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
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Live.Phone;
    using Microsoft.Phone.BackgroundTransfer;

    public class LivePendingUpload
    {
        #region Instance member variables

        private readonly IBackgroundTransferService backgroundTransferService;
        private readonly BackgroundTransferRequest request;

        #endregion

        #region Constructors

        internal LivePendingUpload(
            IBackgroundTransferService backgroundTransferService,
            BackgroundTransferRequest request)
        {
            Debug.Assert(backgroundTransferService != null);
            Debug.Assert(request != null);
            Debug.Assert(BackgroundTransferHelper.IsUploadRequest(request));

            this.backgroundTransferService = backgroundTransferService;
            this.request = request;
        }

        #endregion

        #region Public members

        /// <summary>
        /// Attaches to the upload operation and receive the result of the operation when it is finished.
        /// </summary>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> AttachAsync()
        {
            return this.AttachAsync(new CancellationToken(false), null);
        }

        /// <summary>
        /// Attaches to the upload operation and receive the result of the operation when it is finished.
        /// </summary>
        /// <param name="ct">a token that is used to cancel the background upload operation.</param>
        /// <param name="progress">an object that is called to report the background upload's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> AttachAsync(CancellationToken ct, IProgress<LiveOperationProgress> progress)
        {
            var tcs = new TaskCompletionSource<LiveOperationResult>();

            ct.Register(() =>
            {
                // Remove from the service to cancel.
                this.backgroundTransferService.Remove(this.request);
                tcs.TrySetCanceled();
            });

            var uploadEventHandler = new BackgroundUploadEventAdapter(this.backgroundTransferService, tcs);
            return uploadEventHandler.ConvertTransferStatusChangedToTask(this.request, progress);
        }

        #endregion
    }
}
