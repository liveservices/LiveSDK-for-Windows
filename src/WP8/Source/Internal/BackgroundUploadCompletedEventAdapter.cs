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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Phone.BackgroundTransfer;
    using Microsoft.Live.Operations;

    /// <summary>
    /// Class that converts a Download BackgroundTransferRequest's TransferStatusChanged events
    /// to the Task&lt;T&gt; pattern.
    /// </summary>
    internal class BackgroundUploadCompletedEventAdapter : IBackgroundUploadResponseHandlerObserver
    {
        #region Fields

        private readonly IBackgroundTransferService backgroundTransferService;

        /// <summary>
        /// Used to set the result of the Upload on.
        /// </summary>
        private readonly TaskCompletionSource<LiveOperationResult> tcs;

        #endregion

        #region Events

        public event Action<BackgroundTransferRequest> BackgroundTransferRequestCompleted;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new BackgroundUploadCompletedEventAdapter injected with the given parameters.
        /// </summary>
        /// <param name="backgroundTransferService">The BackgroundTransferService used to Add and Remove requests on.</param>
        /// <param name="tcs">The TaskCompletionSource to set the result of the operation on.</param>
        public BackgroundUploadCompletedEventAdapter(
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
        /// Attaches to this BackgroundTransferRequest's TransferStatusChanged event.
        /// </summary>
        /// <param name="request">request to attach to</param>
        public Task<LiveOperationResult> ConvertTransferStatusChangedToTask(BackgroundTransferRequest request)
        {
            Debug.Assert(BackgroundTransferHelper.IsUploadRequest(request));
            if (request.TransferStatus != TransferStatus.Completed)
            {
                request.TransferStatusChanged += this.HandleTransferStatusChanged;
            }
            else
            {
                // If we are working with an already completed request just handle it now.
                this.OnTransferStatusComplete(request);
            }

            return this.tcs.Task;
        }

        /// <summary>
        /// Callback used when the BackgroundUploadResponseHandler finishes with a success response.
        /// This method will convert the result into a LiveCompletedEventArgs and call OnCompleted.
        /// </summary>
        /// <param name="result">The result dictionary from the upload response.</param>
        /// <param name="rawResult">The raw string from the upload response.</param>
        /// <param name="userState">The userstate of the request.</param>
        public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
        {
            this.tcs.TrySetResult(new LiveOperationResult(result, rawResult));
        }

        /// <summary>
        /// Callback used when the BackgroundUploadResponseHandler finishes with an error in the json response.
        /// This method will conver the error result into a LiveCompletedEventArgs and call OnCompleted.
        /// </summary>
        /// <param name="code">The error code from the error.</param>
        /// <param name="message">The error message from the error.</param>
        public void OnErrorResponse(string code, string message)
        {
            var exception = new LiveConnectException(code, message);
            this.tcs.TrySetException(exception);
        }

        /// <summary>
        /// Callback used when the BackgroundUploadResponseHandler finishes with an error.
        /// </summary>
        /// <param name="exception">The exception from the BackgroundUploadResponseHandler.</param>
        public void OnError(Exception exception)
        {
            var e = new LiveConnectException(ApiOperation.ApiClientErrorCode,
                                             ResourceHelper.GetString("BackgroundUploadResponseHandlerError"),
                                             exception);
            this.tcs.TrySetException(e);
        }

        /// <summary>
        /// Called when a BackgroundTransferRequest's TransferStatus is set to Completed.
        /// This method will remove the request from the BackgroundTransferService and convert
        /// the result over to a LiveOperationResult and set it on the TaskCompletionSource.
        /// </summary>
        /// <param name="request"></param>
        private void OnTransferStatusComplete(BackgroundTransferRequest request)
        {
            Debug.Assert(request.TransferStatus == TransferStatus.Completed);
            Debug.Assert(BackgroundTransferHelper.IsUploadRequest(request));

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

            this.OnBackgroundTransferRequestCompleted(request);

            if (request.TransferError != null)
            {
                var exception = new LiveConnectException(ApiOperation.ApiServerErrorCode,
                                                         ResourceHelper.GetString("ServerError"),
                                                         request.TransferError);
                this.tcs.TrySetException(exception);
                return;
            }

            if (!BackgroundTransferHelper.IsSuccessfulStatusCode(request.StatusCode))
            {
                var exception = new LiveConnectException(ApiOperation.ApiServerErrorCode,
                                                         ResourceHelper.GetString("ServerError"));
                this.tcs.TrySetException(exception);
                return;
            }

            // Once we know we have a *good* upload, we have to send it to the response handler
            // to read it's JSON response body. We are an observer to this class, so it will call us back
            // with its result.
            var responseHandler = new BackgroundUploadResponseHandler(
                request.DownloadLocation,
                this);
            responseHandler.ReadJsonResponseFromDownloadLocation();
        }

        /// <summary>
        /// Callback used to listen to BackgroundTransferRequest.TransferStatusChanged events.
        /// This method ignores all TransferStatus changes except for TransferStatus.Completed.
        /// </summary>
        /// <param name="sender">The sender of the call.</param>
        /// <param name="e">The eventargs from the BackgroundTransferRequest.</param>
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

        private void OnBackgroundTransferRequestCompleted(BackgroundTransferRequest request)
        {
            Action<BackgroundTransferRequest> handler = BackgroundTransferRequestCompleted;
            if (handler != null)
            {
                handler(request);
            }
        }

        #endregion
    }
}
