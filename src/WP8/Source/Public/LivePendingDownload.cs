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

namespace Microsoft.Live
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Live.Phone;
    using Microsoft.Phone.BackgroundTransfer;

    public class LivePendingDownload
    {
        #region Instance member variables

        private readonly IBackgroundTransferService backgroundTransferService;
        private readonly BackgroundTransferRequest request;

        #endregion

        #region Constructors

        internal LivePendingDownload(
            IBackgroundTransferService backgroundTransferService, 
            BackgroundTransferRequest request)
        {
            Debug.Assert(backgroundTransferService != null);
            Debug.Assert(request != null);
            Debug.Assert(BackgroundTransferHelper.IsDownloadRequest(request));

            this.request = request;
            this.backgroundTransferService = backgroundTransferService;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Attaches to the download operation and receive the result of the operation when it is finished.
        /// </summary>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> AttachAsync()
        {
            return this.AttachAsync(new CancellationToken(false), null);
        }

        /// <summary>
        /// Attaches to the pending download and receive the result of the operation when it is finished.
        /// </summary>
        /// <param name="ct">a token that is used to cancel the background download operation.</param>
        /// <param name="progress">an object that is called to report the background download's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> AttachAsync(CancellationToken ct, IProgress<LiveOperationProgress> progress)
        {
            var tcs = new TaskCompletionSource<LiveOperationResult>();
            ct.Register(() =>
            {
                // remove from service to cancel.
                this.backgroundTransferService.Remove(this.request);
                tcs.TrySetCanceled();
            });

            var downloadEventAdapter = new BackgroundDownloadEventAdapter(this.backgroundTransferService, tcs);
            return downloadEventAdapter.ConvertTransferStatusChangedToTask(this.request, progress);
        }

        #endregion
    }
}
