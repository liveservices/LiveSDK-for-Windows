namespace Microsoft.Live.Phone
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This class combines the BackgroundDownloadCompletedEventAdapter and BackgroundDownloadProgressEventAdapter
    /// into one class.
    /// </summary>
    internal class BackgroundDownloadEventAdapter
    {
        #region Fields

        private readonly IBackgroundTransferService backgroundTransferService;

        /// <summary>
        /// Passed to BackgroundDownloadCompletedEventAdapter so it can set the result of the download task.
        /// </summary>
        private readonly TaskCompletionSource<LiveOperationResult> tcs; 

        private BackgroundDownloadProgressEventAdapter progressEventAdapter;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new BackgroundUploadEventAdapter injected with the given parameters.
        /// </summary>
        /// <param name="backgroundTransferService">The BackgroundTransferService used to Add and Remove requests on.</param>
        /// <param name="tcs">The TaskCompletionSource to set the result of the operation on.</param>
        public BackgroundDownloadEventAdapter(
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
        /// Attaches a BackgroundDownloadCompletedEventAdapter to the given BackgroundTransferRequest.
        /// This is to convert a BackgroundTransferRequest's status changes to a LiveOperationResult.
        /// </summary>
        /// <param name="request">Request to attach to.</param>
        /// <returns>A Task&lt;LiveOperationResult&gt; converted over from a BackgroundTransferEventArgs.</returns>
        public Task<LiveOperationResult> ConvertTransferStatusChangedToTask(BackgroundTransferRequest request)
        {
            Debug.Assert(request != null);

            var completedEventAdapter =
                new BackgroundDownloadCompletedEventAdapter(this.backgroundTransferService, this.tcs);

            completedEventAdapter.BackgroundTransferRequestCompleted +=
                this.OnBackgroundTransferRequestCompletedEventAdapter;
            return completedEventAdapter.ConvertTransferStatusChangedToTask(request);
        }

        /// <summary>
        /// Attaches a BackgroundDownloadCompletedEventAdapter and a BackgroundDownloadProgressEventAdapter 
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
                this.progressEventAdapter = new BackgroundDownloadProgressEventAdapter(progress);
                this.progressEventAdapter.ConvertTransferProgressChanged(request);
            }

            return this.ConvertTransferStatusChangedToTask(request);
        }

        /// <summary>
        /// Callback used when BackgroundTransferRequest's TransferStatus changes to Completed.
        /// This method performs clean up and detaches the progressEventAdapter event adapter from the request.
        /// </summary>
        private void OnBackgroundTransferRequestCompletedEventAdapter(BackgroundTransferRequest request)
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
