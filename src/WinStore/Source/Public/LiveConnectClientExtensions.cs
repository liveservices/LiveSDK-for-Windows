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

namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
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

        #endregion

        #region Public methods

        /// <summary>
        /// Gets current active download operations
        /// </summary>
        /// <returns>A collection of currently pending LiveDownloadOperation instances</returns>
        public static async Task<IEnumerable<LiveDownloadOperation>> GetCurrentBackgroundDownloadsAsync()
        {
            var pendingOperations = new LinkedList<LiveDownloadOperation>();
            var activeDownloads = await BT.BackgroundDownloader.GetCurrentDownloadsAsync(LiveSDKDownloadGroup);
            foreach (BT.DownloadOperation downloadOp in activeDownloads)
            {
                pendingOperations.AddLast(new LiveDownloadOperation(downloadOp));
            }

            return pendingOperations;
        }


        /// <summary>
        /// Gets current active upload operations
        /// </summary>
        /// <returns>A collection of currently pending LiveUploadOperation instances</returns>
        public static async Task<IEnumerable<LiveUploadOperation>> GetCurrentBackgroundUploadsAsync()
        {
            var pendingOperations = new LinkedList<LiveUploadOperation>();
            var activeUploads = await BT.BackgroundUploader.GetCurrentUploadsAsync(LiveSDKUploadGroup);
            foreach (BT.UploadOperation uploadOp in activeUploads)
            {
                pendingOperations.AddLast(new LiveUploadOperation(uploadOp));
            }

            return pendingOperations;
        }

        /// <summary>
        /// Creates background download operation.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public Task<LiveDownloadOperation> CreateBackgroundDownloadAsync(string path, IStorageFile outputFile = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            var op = new CreateBackgroundDownloadOperation(
                    this,
                    this.GetResourceUri(path, ApiMethod.Download),
                    outputFile);

            return op.ExecuteAsync();
        }

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

        /// <summary>
        /// Creates a background upload operation.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputFile">the file object of the local file to be uploaded.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveUploadOperation> CreateBackgroundUploadAsync(
            string path,
            string fileName,
            IStorageFile inputFile,
            OverwriteOption option)
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

            var op = new CreateBackgroundUploadOperation(
                    this,
                    this.GetResourceUri(path, ApiMethod.Upload),
                    fileName,
                    inputFile,
                    option);
            return op.ExecuteAsync();
        }

        /// <summary>
        /// Creates a background upload operation.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the upload's content.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveUploadOperation> CreateBackgroundUploadAsync(
            string path,
            string fileName,
            IInputStream inputStream,
            OverwriteOption option)
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

            var op = new CreateBackgroundUploadOperation(
                    this,
                    this.GetResourceUri(path, ApiMethod.Upload),
                    fileName,
                    inputStream,
                    option);
            return op.ExecuteAsync();
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
        /// <returns>An async task returning a LiveDownloadOperationResult instance.</returns>
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
