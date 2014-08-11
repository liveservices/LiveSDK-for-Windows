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
    using System.Threading.Tasks;

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This class combines the BackgroundUploadCompletedEventAdapter and BackgroundUploadProgressEventAdapter
    /// into one class.
    /// </summary>
    internal class BackgroundUploadEventAdapter
    {
        #region Fields

        private readonly IBackgroundTransferService backgroundTransferService;

        /// <summary>
        /// Passed to BackgroundUploadCompletedEventAdapter so it can set the result of the upload task.
        /// </summary>
        private readonly TaskCompletionSource<LiveOperationResult> tcs; 

        private BackgroundUploadProgressEventAdapter progressEventAdapter;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new BackgroundUploadEventAdapter injected with the given parameters.
        /// </summary>
        /// <param name="backgroundTransferService">The BackgroundTransferService used to Add and Remove requests on.</param>
        /// <param name="tcs">The TaskCompletionSource to set the result of the operation on.</param>
        public BackgroundUploadEventAdapter(
            IBackgroundTransferService backgroundTransferService,
            TaskCompletionSource<LiveOperationResult> tcs)
        {
            Debug.Assert(backgroundTransferService != null);
            Debug.Assert(tcs != null);

            this.backgroundTransferService = backgroundTransferService;
            this.tcs = tcs;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches a BackgroundUploadCompletedEventAdapter to the given BackgroundTransferRequest.
        /// This is to convert a BackgroundTransferRequest's status changes to a LiveOperationResult.
        /// </summary>
        /// <param name="request">Request to attach to.</param>
        /// <returns>A Task&lt;LiveOperationResult&gt; converted over from a BackgroundTransferEventArgs.</returns>
        public Task<LiveOperationResult> ConvertTransferStatusChangedToTask(BackgroundTransferRequest request)
        {
            var completedEventHandler =
                new BackgroundUploadCompletedEventAdapter(this.backgroundTransferService, this.tcs);
            completedEventHandler.BackgroundTransferRequestCompleted += this.OnBackgroundTransferRequestCompleted;

            return completedEventHandler.ConvertTransferStatusChangedToTask(request);
        }

        /// <summary>
        /// Attaches a BackgroundUploadCompletedEventAdapter and a BackgroundUploadProgressEventAdapter 
        /// to the given BackgroundTransferRequest.
        /// This is to convert a BackgroundTransferRequest's status changes to a LiveOperationResult and 
        /// to change it's progress changes to a LiveOperationProgress.
        /// </summary>
        /// <param name="request">Request to attach to.</param>
        /// <param name="progress">The interface to call when there is a progress event.</param>
        /// <returns>A Task&lt;LiveOperationResult&gt; converted over from a BackgroundTransferEventArgs.</returns>
        public Task<LiveOperationResult> ConvertTransferStatusChangedToTask(
            BackgroundTransferRequest request,
            IProgress<LiveOperationProgress> progress)
        {
            if (progress != null)
            {
                this.progressEventAdapter = new BackgroundUploadProgressEventAdapter(progress);
                this.progressEventAdapter.ConvertTransferProgressChanged(request);
            }

            return this.ConvertTransferStatusChangedToTask(request);
        }

        /// <summary>
        /// Callback used when BackgroundTransferRequest's TransferStatus changes to Completed.
        /// This method performs clean up and detaches the progressEventAdapter event adapter from the request.
        /// </summary>
        private void OnBackgroundTransferRequestCompleted(BackgroundTransferRequest request)
        {
            // this.completedEventAdapter detaches itself.
            if (this.progressEventAdapter != null)
            {
                this.progressEventAdapter.DetachFromCompletedRequest(request);
            }
        }

        #endregion
    }
}
