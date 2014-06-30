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

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This class attaches to a BackgroundTransferRequest and converts its ProgressChanged events
    /// to LiveOperationProgresses and forwards them to a IProgress interface.
    /// </summary>
    internal class BackgroundUploadProgressEventAdapter
    {
        #region Fields

        private readonly IProgress<LiveOperationProgress> progress;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new eventadapter that forwards events to the given event handler.
        /// </summary>
        public BackgroundUploadProgressEventAdapter(IProgress<LiveOperationProgress> progress)
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
            Debug.Assert(BackgroundTransferHelper.IsUploadRequest(request));
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
            long totalBytesToSend = request.TotalBytesToSend;
            long bytesSent = request.BytesSent;
            var result = new LiveOperationProgress(bytesSent, totalBytesToSend);
            this.OnProgressChanged(result);
        }

        private void OnProgressChanged(LiveOperationProgress e)
        {
            this.progress.Report(e);
        }

        #endregion
    }
}
