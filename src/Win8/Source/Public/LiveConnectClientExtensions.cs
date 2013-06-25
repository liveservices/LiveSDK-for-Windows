namespace Microsoft.Live
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using BT = Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;

    using Microsoft.Live.Operations;

    /// <summary>
    /// This class contains the Windows 8 specific public extension methods for the LiveConnectClient class.
    /// </summary>
    public partial class LiveConnectClient
    {
        #region Internal Static members

        /// <summary>
        /// A constant used by BackgroundDownloader to identify operations stared via the Live SDK.
        /// </summary>
        internal static readonly string LiveSDKDownloadGroup = "LiveSDKDownloadGroup_C8B9A6E4-1030-4BE1-ADB6-1FB89AFDCED3";

        /// <summary>
        /// A constant used by BackgroundUploader to identify operations stared via the Live SDK.
        /// </summary>
        internal static readonly string LiveSDKUploadGroup = "LiveSDKUploadGroup_C8B9A6E4-1030-4BE1-ADB6-1FB89AFDCED3";

        static LiveConnectClient()
        {
            LiveConnectClient.AttachActiveTransfers();
        }

        static async void AttachActiveTransfers()
        {
            var activeDownloads = await BT.BackgroundDownloader.GetCurrentDownloadsAsync(LiveSDKDownloadGroup);
            foreach (BT.DownloadOperation downloadOp in activeDownloads)
            {
                new TailoredDownloadOperation().Attach(downloadOp);
            }

            var activeUploads = await BT.BackgroundUploader.GetCurrentUploadsAsync(LiveSDKUploadGroup);
            foreach (BT.UploadOperation uploadOp in activeUploads)
            {
                new TailoredUploadOperation().Attach(uploadOp);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Download a file to disk.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="outputFile">the file object that the downloaded content is written to.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(string path, IStorageFile outputFile)
        {
            return this.BackgroundDownloadAsync(path, outputFile, new CancellationToken(false), null);
        }

        /// <summary>
        /// Download a file to disk.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="outputFile">the file that the downloaded content is written to.</param>
        /// <param name="ct">a token that is used to cancel the download operation.</param>
        /// <param name="progress">an object that is called to report the download's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(
            string path, 
            IStorageFile outputFile, 
            CancellationToken ct, 
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), "path"));
            }

            if (outputFile == null)
            {
                throw new ArgumentNullException("outputFile",
                   String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), "outputFile"));
            }

            return this.InternalDownloadAsync(path, outputFile, ct, progress);
        }


        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(string path)
        {
            return this.BackgroundDownloadAsync(path, new CancellationToken(false), null);
        }

        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="ct">a token that is used to cancel the upload operation.</param>
        /// <param name="progress">an object that is called to report the download's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(
            string path,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "path",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), "path"));
            }

            return this.InternalDownloadAsync(path, null, ct, progress);
        }

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputFile">the file object of the local file to be uploaded.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path, 
            string fileName, 
            IStorageFile inputFile, 
            OverwriteOption option)
        {
            return this.BackgroundUploadAsync(path, fileName, inputFile, option, new CancellationToken(false), null);
        }

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputFile">the file object of the local file to be uploaded.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <param name="ct">a token that is used to cancel the upload operation.</param>
        /// <param name="progress">an object that is called to report the upload's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path,
            string fileName, 
            IStorageFile inputFile,
            OverwriteOption option, 
            CancellationToken ct, 
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), "path"));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"), "fileName"));
            }

            if (inputFile == null)
            {
                throw new ArgumentNullException("inputFile",
                   String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), "inputFile"));
            }

            ApiOperation op = new TailoredUploadOperation(
                this, 
                this.GetResourceUri(path, ApiMethod.Upload), 
                fileName, 
                inputFile,
                option,
                progress, 
                null);

            return this.ExecuteApiOperation(op, ct);
        }

        /// <summary>
        /// Upload a stream to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the upload's content.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path,
            string fileName,
            IInputStream inputStream,
            OverwriteOption option)
        {
            return this.BackgroundUploadAsync(path, fileName, inputStream, option, new CancellationToken(false), null);
        }

        /// <summary>
        /// Upload a stream to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the upload's content.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <param name="ct">a token that is used to cancel the upload operation.</param>
        /// <param name="progress">an object that is called to report the upload's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path,
            string fileName,
            IInputStream inputStream,
            OverwriteOption option,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(
                    "path",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"),
                    "path"));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(
                    "fileName",
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"),
                    "fileName"));
            }

            if (inputStream == null)
            {
                throw new ArgumentNullException(
                    "inputStream",
                   String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"),
                   "inputStream"));
            }

            ApiOperation op =
                new TailoredUploadOperation(
                    this,
                    this.GetResourceUri(path, ApiMethod.Upload),
                    fileName,
                    inputStream,
                    option,
                    progress,
                    null);

            return this.ExecuteApiOperation(op, ct);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Download a file to disk.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="outputFile">the file that the downloaded content is written to.</param>
        /// <param name="ct">a token that is used to cancel the download operation.</param>
        /// <param name="progress">a delegate that is called to report the download progress.</param>
        /// <returns>TODO</returns>
        internal Task<LiveDownloadOperationResult> InternalDownloadAsync(
            string path, 
            IStorageFile outputFile, 
            CancellationToken ct, 
            IProgress<LiveOperationProgress> progress)
        {
            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var tcs = new TaskCompletionSource<LiveDownloadOperationResult>();

            var op = new TailoredDownloadOperation(
                    this, 
                    this.GetResourceUri(path, ApiMethod.Download), 
                    outputFile, 
                    progress, 
                    null);

            op.OperationCompletedCallback = (LiveDownloadOperationResult result) =>
            {
                if (result.IsCancelled)
                {
                    tcs.TrySetCanceled();
                }
                else if (result.Error != null)
                {
                    tcs.TrySetException(result.Error);
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            };

            ct.Register(op.Cancel);

            op.Execute();

            return tcs.Task;
        }

        #endregion
    }
}
