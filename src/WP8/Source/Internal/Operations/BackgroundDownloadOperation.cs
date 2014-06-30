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

namespace Microsoft.Live.Phone.Operations
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This class creates a BackgroundTransferRequest for downloads, sends the request to the BackgroundTransferService
    /// and forwards any events.
    /// </summary>
    internal class BackgroundDownloadOperation
    {
        #region Nested types

        /// <summary>
        /// Class used to build a BackgroundDownloadOperation because of its long list of parameters.
        /// </summary>
        public class Builder
        {
            private Uri requestUri;
            private Uri downloadLocationOnDevice;
            private string accessToken;
            private IBackgroundTransferService backgroundTransferService;

            public Builder()
            {
                this.backgroundTransferService = PhoneBackgroundTransferService.Instance;
            }

            public Uri RequestUri
            {
                get
                {
                    return this.requestUri;
                }
                set
                {
                    Debug.Assert(value != null);
                    this.requestUri = value;
                }
            }
            
            public string AccessToken
            {
                get
                {
                    return this.accessToken;
                }
                set
                {
                    Debug.Assert(value != null);
                    this.accessToken = value;
                }
            }

            public Uri DownloadLocationOnDevice
            {
                get
                {
                    return this.downloadLocationOnDevice;
                }
                set
                {
                    Debug.Assert(value != null);
                    this.downloadLocationOnDevice = value;
                }
            }

            public IBackgroundTransferService BackgroundTransferService
            {
                get
                {
                    return this.backgroundTransferService;
                }
                set
                {
                    Debug.Assert(value != null);
                    this.backgroundTransferService = value;
                }
            }

            public IProgress<LiveOperationProgress> Progress { get; set; }

            public BackgroundTransferPreferences BackgroundTransferPreferences { get; set; }

            public BackgroundDownloadOperation Build()
            {
                Debug.Assert(this.requestUri != null);
                Debug.Assert(this.accessToken != null);
                Debug.Assert(this.downloadLocationOnDevice != null);
                Debug.Assert(this.backgroundTransferService != null);

                return new BackgroundDownloadOperation(this);
            }
        }

        #endregion

        #region Fields

        private readonly Uri requestUri;
        private readonly string accessToken;
        private readonly Uri downloadLocationOnDevice;
        private readonly IBackgroundTransferService backgroundTransferService;
        private readonly IProgress<LiveOperationProgress> progress;
        private readonly TaskCompletionSource<LiveOperationResult> tcs;
        private readonly TransferPreferences transferPreferences;

        private BackgroundTransferRequest request;
        private OperationStatus status;

        #endregion

        #region Constructors

        private BackgroundDownloadOperation(Builder builder)
        {
            this.requestUri = builder.RequestUri;
            this.accessToken = builder.AccessToken;
            this.downloadLocationOnDevice = builder.DownloadLocationOnDevice;
            this.backgroundTransferService = builder.BackgroundTransferService;
            this.progress = builder.Progress;
            this.tcs = new TaskCompletionSource<LiveOperationResult>();
            this.status = OperationStatus.NotStarted;
            this.transferPreferences =
                BackgroundTransferHelper.GetTransferPreferences(builder.BackgroundTransferPreferences);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancels the given operation.
        /// </summary>
        public void Cancel()
        {
            // If we are already cancelled or completed we can just leave.
            if (this.status == OperationStatus.Cancelled || this.status == OperationStatus.Completed)
            {
                return;
            }

            // if we have started we must remove the request from the service to cancel.
            if (this.status == OperationStatus.Started)
            {
                this.backgroundTransferService.Remove(this.request);
            }

            // if we have not started, or started we must switch the state to cancelled and
            // notify the TaskCompletionSource to cancel.
            this.status = OperationStatus.Cancelled;
            this.tcs.TrySetCanceled();
        }

        /// <summary>
        /// Performs the BackgroundDownloadOperation.
        /// </summary>
        public async Task<LiveOperationResult> ExecuteAsync()
        {
            Debug.Assert(this.status != OperationStatus.Completed, "Cannot execute on a completed operation.");

            var builder = new BackgroundDownloadRequestBuilder
            {
                AccessToken = this.accessToken,
                DownloadLocationOnDevice = this.downloadLocationOnDevice,
                RequestUri = this.requestUri,
                TransferPreferences = this.transferPreferences
            };

            this.request = builder.Build();
            var eventAdapter = new BackgroundDownloadEventAdapter(this.backgroundTransferService, this.tcs);

            Task<LiveOperationResult> task = this.progress == null ?
                                             eventAdapter.ConvertTransferStatusChangedToTask(this.request) :
                                             eventAdapter.ConvertTransferStatusChangedToTask(this.request, this.progress);

            Debug.Assert(this.tcs.Task == task, "EventAdapter returned a different task. This could affect cancel.");

            // if the request has already been cancelled do not add it to the service.
            if (this.status != OperationStatus.Cancelled)
            {
                this.backgroundTransferService.Add(this.request);
                this.status = OperationStatus.Started;
            }

            LiveOperationResult result = await task;
            this.status = OperationStatus.Completed;
            return result;
        }

        #endregion
    }
}
