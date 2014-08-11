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

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This class attaches to a BackgroundTransferRequest and converts its ProgressChanged events
    /// to LiveOperationProgresses and forwards them to a IProgress interface.
    /// </summary>
    internal class BackgroundDownloadProgressEventAdapter 
    {
        private readonly IProgress<LiveOperationProgress> progress;

        #region Constructors

        /// <summary>
        /// Constructs a new eventadapter that forwards events to the callback.
        /// </summary>
        /// <param name="progress">the callback that receives the forwarded events</param>
        public BackgroundDownloadProgressEventAdapter(IProgress<LiveOperationProgress> progress)
        {
            Debug.Assert(progress != null);
            this.progress = progress;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches to the request's TransferProgressChanged event.
        /// It then converts these events to LiveOperationProgresses and gives them to the IProgress
        /// interface that the object was constructed with.
        /// NOTE: Call DetachFromCompletedRequest from this instance when the given request is Completed.
        /// </summary>
        /// <param name="request">request to attach to</param>
        public void ConvertTransferProgressChanged(BackgroundTransferRequest request)
        {
            Debug.Assert(BackgroundTransferHelper.IsDownloadRequest(request));
            request.TransferProgressChanged += this.HandleTransferProgressChanged;
        }

        /// <summary>
        /// Detaches from the request's TransferProgressChanged event.
        /// NOTE: The request should have its TransferStatus set to Completed before detaching.
        /// This method should be called as part of cleaning up a completed request.
        /// </summary>
        /// <param name="request">request to detach to</param>
        public void DetachFromCompletedRequest(BackgroundTransferRequest request)
        {
            Debug.Assert(request.TransferStatus == TransferStatus.Completed);
            request.TransferProgressChanged -= this.HandleTransferProgressChanged;
        }

        /// <summary>
        /// Handles TransferProgressChanged events from a BackgroundTransferRequest.
        /// </summary>
        private void HandleTransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            BackgroundTransferRequest request = e.Request;
            long totalBytesToReceive = request.TotalBytesToReceive;
            long bytesReceived = request.BytesReceived;
            var args = new LiveOperationProgress(bytesReceived, totalBytesToReceive);
            this.OnProgressChanged(args);
        }

        private void OnProgressChanged(LiveOperationProgress e)
        {
            this.progress.Report(e);
        }

        #endregion
    }
}
