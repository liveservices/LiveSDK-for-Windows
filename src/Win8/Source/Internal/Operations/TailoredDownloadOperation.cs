namespace Microsoft.Live.Operations
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using BT = Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;
    
    /// <summary>
    /// This class implements the download operation on Windows 8 using the 
    /// background downloader.
    /// </summary>
    internal class TailoredDownloadOperation : ApiOperation
    {
        #region Instance member variables

        private const uint MaxDownloadResponseLength = 10240;
        private BT.DownloadOperation downloadOp;
        private CancellationTokenSource cts;
        private bool isAttach;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new TailoredDownloadOperation instance.
        /// </summary>
        public TailoredDownloadOperation(
            LiveConnectClient client, 
            Uri url, 
            IStorageFile outputFile, 
            IProgress<LiveOperationProgress> progress,
            SynchronizationContextWrapper syncContext)
            : base(client, url, ApiMethod.Download, null, syncContext)
        {
            this.OutputFile = outputFile;
            this.Progress = progress;
        }

        /// <summary>
        /// Creates a new TailoredDownloadOperation instance.
        /// </summary>
        /// <remarks>This constructor should only be used when attaching to an pending download.</remarks>
        internal TailoredDownloadOperation()
            : base(null, null, ApiMethod.Download, null, null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the IStorageFile object that the downloaded content is to be stored in.
        /// </summary>
        public IStorageFile OutputFile { get; private set; }

        /// <summary>
        /// Gets the download progress handler.
        /// </summary>
        /// <remarks>This is the progress handler for Task async pattern.</remarks>
        public IProgress<LiveOperationProgress> Progress { get; private set; }

        /// <summary>
        /// Gets the operation completed callback.
        /// </summary>
        public new Action<LiveDownloadOperationResult> OperationCompletedCallback { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Attach to a pending download operation.
        /// </summary>
        public async void Attach(BT.DownloadOperation downloadOp)
        {
            Debug.Assert(downloadOp != null);

            this.downloadOp = downloadOp;
            this.isAttach = true;
            this.cts = new CancellationTokenSource();
            var progressHandler = new Progress<BT.DownloadOperation>(this.OnDownloadProgress);

            try
            {
                // Since we don't provide API for apps to attach to pending download operations, we have no
                // way to invoke the app event handler to provide any feedback. We would just ignore the result here.
                this.downloadOp = await this.downloadOp.AttachAsync().AsTask(this.cts.Token, progressHandler);
            }
            catch
            {
                // Ignore errors as well. 
            }
        }

        /// <summary>
        /// Cancel the download operation.
        /// </summary>
        public override void Cancel()
        {
            if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
            {
                return;
            }

            this.Status = OperationStatus.Cancelled;

            if (this.downloadOp != null)
            {
                this.cts.Cancel();
            }
            else
            {
                this.OnCancel();
            }
        }

        #endregion

        #region Protected methods

        protected override void OnCancel()
        {
            this.OnOperationCompleted(new LiveDownloadOperationResult(null, true));
        }

        /// <summary>
        /// Overrides the base execute logic to use the background downloader to download the file.
        /// </summary>
        protected async override void OnExecute()
        {
            BT.BackgroundDownloader downloader;
            if (this.Url.OriginalString.StartsWith(this.LiveClient.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                if (base.RefreshTokenIfNeeded())
                {
                    return;
                }

                downloader = new BT.BackgroundDownloader();
                if (this.LiveClient.Session != null)
                {
                    downloader.SetRequestHeader(
                        ApiOperation.AuthorizationHeader,
                        AuthConstants.BearerTokenType + " " + this.LiveClient.Session.AccessToken);
                }
                downloader.SetRequestHeader(ApiOperation.LibraryHeader, Platform.GetLibraryHeaderValue());
            }
            else
            {
                downloader = new BT.BackgroundDownloader();
            }

            downloader.Group = LiveConnectClient.LiveSDKDownloadGroup;
            this.cts = new CancellationTokenSource();
            this.downloadOp = downloader.CreateDownload(this.Url, this.OutputFile);
            var progressHandler = new Progress<BT.DownloadOperation>(this.OnDownloadProgress);

            LiveDownloadOperationResult result = null;
            Exception webError = null;
            try
            {
                this.downloadOp = await this.downloadOp.StartAsync().AsTask(this.cts.Token, progressHandler);

                result = this.OutputFile != null ?
                         new LiveDownloadOperationResult(this.OutputFile) :
                         new LiveDownloadOperationResult(this.downloadOp.GetResultStreamAt(0));
            }
            catch (TaskCanceledException)
            {
                result = new LiveDownloadOperationResult(null, true);
            }
            catch (Exception error)
            {
                webError = error;
            }

            if (webError != null)
            {
                result = await this.ProcessDownloadErrorResponse(webError);
            }

            this.OnOperationCompleted(result);
        }

        /// <summary>
        /// Calls the OperationCompletedCallback with the given result.
        /// This should be called when the TailoredDownloadOperation is completed.
        /// </summary>
        protected void OnOperationCompleted(LiveDownloadOperationResult result)
        {
            Action<LiveDownloadOperationResult> callback = this.OperationCompletedCallback;
            if (callback != null)
            {
                callback(result);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Called from the BTS.DownloadOperation progress event and raise this's download progress.
        /// </summary>
        private void OnDownloadProgress(BT.DownloadOperation downloadOp)
        {
            Debug.Assert(downloadOp != null);

            if (downloadOp.Progress.Status != BT.BackgroundTransferStatus.Error &&
                downloadOp.Progress.Status != BT.BackgroundTransferStatus.Canceled)
            {
                if (!this.isAttach)
                {
                    if (this.Progress != null)
                    {
                        this.Progress.Report(
                            new LiveOperationProgress(
                                (long)downloadOp.Progress.BytesReceived,
                                (long)downloadOp.Progress.TotalBytesToReceive));
                    }
                }
            }
        }

        /// <summary>
        /// Process the error response.
        /// </summary>
        private async Task<LiveDownloadOperationResult> ProcessDownloadErrorResponse(Exception exception)
        {
            LiveDownloadOperationResult opResult;
            try
            {
                IInputStream responseStream = this.downloadOp.GetResultStreamAt(0);
                if (responseStream == null)
                {
                    var error = new LiveConnectException(
                        ApiOperation.ApiServerErrorCode,
                        ResourceHelper.GetString("ConnectionError"));
                    opResult = new LiveDownloadOperationResult(error, false);
                }
                else
                {
                    var reader = new DataReader(responseStream);
                    uint length = await reader.LoadAsync(TailoredDownloadOperation.MaxDownloadResponseLength);
                    Exception error = this.CreateOperationResultFrom(reader.ReadString(length)).Error;
                    if (error is FormatException)
                    {
                        // if we cannot understand the error response,
                        // return the exception thrown by the background downloader.
                        error = exception;
                    }

                    opResult = new LiveDownloadOperationResult(error, false);
                }
            }
            catch (COMException exp)
            {
                opResult = new LiveDownloadOperationResult(exp, false);
            }
            catch (FileNotFoundException exp)
            {
                opResult = new LiveDownloadOperationResult(exp, false);
            }

            return opResult;
        }

        #endregion
    }
}
