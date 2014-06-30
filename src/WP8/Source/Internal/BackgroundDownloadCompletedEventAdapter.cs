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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Live.Operations;
    using Microsoft.Live.Serialization;
    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// Class that converts a Download BackgroundTransferRequest's TransferStatusChanged events
    /// to the Task&lt;T&gt; pattern.
    /// </summary>
    internal class BackgroundDownloadCompletedEventAdapter
    {
        #region Constants

        private const string JsonResponse = @"{{""downloadLocation"": ""{0}""}}";

        #endregion

        #region Fields

        /// <summary>
        /// Used to set the result of the Download on.
        /// </summary>
        private readonly TaskCompletionSource<LiveOperationResult> tcs; 

        private readonly IBackgroundTransferService backgroundTransferService;

        #endregion

        #region Events

        /// <summary>
        /// Event fired when the BackgroundTransferRequest's TransferStatus is Completed.
        /// </summary>
        public event Action<BackgroundTransferRequest> BackgroundTransferRequestCompleted;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new BackgroundDownloadCompletedEventAdapter injected with the given parameters.
        /// </summary>
        /// <param name="backgroundTransferService">The BackgroundTransferService used to Add and Remove requests on.</param>
        /// <param name="tcs">The TaskCompletionSource to set the result of the operation on.</param>
        public BackgroundDownloadCompletedEventAdapter(
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
        /// Attaches to the BackgroundTransferRequest's TransferStatus change and converts it into using
        /// a Task&lt;Tgt;
        /// </summary>
        /// <param name="request">To attach to.</param>
        /// <returns>Task&lt;LiveOperationResult&gt; converted from BackgroundTransferEventArgs.</returns>
        public Task<LiveOperationResult> ConvertTransferStatusChangedToTask(BackgroundTransferRequest request)
        {
            Debug.Assert(BackgroundTransferHelper.IsDownloadRequest(request));

            if (request.TransferStatus != TransferStatus.Completed)
            {
                request.TransferStatusChanged += this.HandleTransferStatusChanged;
            }
            else
            {
                // If we are working with an already completed request just handle it now.
                this.OnTransferStatusComplete(request);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Called when a BackgroundTransferRequet's TransferStatus is set to Completed.
        /// This method will remove the request from the BackgroundTransferService and call
        /// the LiveOperationEventArgs event handler.
        /// </summary>
        /// <param name="request">request with a TransferStatus that is set to Completed</param>
        private void OnTransferStatusComplete(BackgroundTransferRequest request)
        {
            Debug.Assert(request.TransferStatus == TransferStatus.Completed);
            Debug.Assert(BackgroundTransferHelper.IsDownloadRequest(request));

            this.OnBackgroundTransferRequestCompleted(request);

            // Remove the transfer request in order to make room in the queue for more transfers.
            // Transfers are not automatically removed by the system.
            // Cancelled requests have already been removed from the system and cannot be removed twice.
            if (!BackgroundTransferHelper.IsCanceledRequest(request))
            {
                try
                {
                    this.backgroundTransferService.Remove(request);
                }
                catch (Exception exception)
                {
                    this.tcs.TrySetException(new LiveConnectException(
                        ApiOperation.ApiClientErrorCode,
                        ResourceHelper.GetString("BackgroundTransferServiceRemoveError"),
                        exception));
                    return;
                }
            }

            if (request.TransferError != null)
            {
                var exception = new LiveConnectException(ApiOperation.ApiServerErrorCode,
                                                         ResourceHelper.GetString("ServerError"),
                                                         request.TransferError);

                this.tcs.TrySetException(exception);
            }
            else if (!BackgroundTransferHelper.IsSuccessfulStatusCode(request.StatusCode))
            {
                var exception = new LiveConnectException(ApiOperation.ApiServerErrorCode,
                                                         ResourceHelper.GetString("ServerError"));
                this.tcs.TrySetException(exception);
            }
            else
            {
                string jsonResponse = string.Format(JsonResponse, request.DownloadLocation.OriginalString.Replace("\\", "\\\\"));
                var jsonReader = new JsonReader(jsonResponse);
                var jsonObject = jsonReader.ReadValue() as IDictionary<string, object>;
                var result = new LiveOperationResult(jsonObject, jsonResponse);

                this.tcs.TrySetResult(result);
            }
        }

        /// <summary>
        /// Callback used when the requests TransferStatusChanged event fires.
        /// Checks to see if the status changed to Completed and calls OnTransferStatusComplete
        /// if it did.
        /// </summary>
        private void HandleTransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            BackgroundTransferRequest request = e.Request;
            if (request.TransferStatus != TransferStatus.Completed)
            {
                return;
            }

            request.TransferStatusChanged -= this.HandleTransferStatusChanged;

            this.OnTransferStatusComplete(request);
        }

        /// <summary>
        /// Method to send out an event that the BackgroundTransferRequest is completed.
        /// </summary>
        private void OnBackgroundTransferRequestCompleted(BackgroundTransferRequest request)
        {
            Action<BackgroundTransferRequest> handler = this.BackgroundTransferRequestCompleted;
            if (handler != null)
            {
                handler(request);
            }
        }

        #endregion
    }
}
