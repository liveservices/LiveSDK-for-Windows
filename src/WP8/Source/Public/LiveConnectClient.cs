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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Live.Phone;
    using Microsoft.Live.Phone.Operations;
    using Microsoft.Live.Operations;
    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// This is the class that applications use to interact with the Api service.
    /// </summary>
    public sealed partial class LiveConnectClient
    {
        #region Fields

        private IBackgroundTransferService backgroundTransferService;

        #endregion

        #region Properties

        /// <summary>
        /// Settings for when Background Transfers are allowed to execute.
        /// </summary>
        public BackgroundTransferPreferences BackgroundTransferPreferences { get; set; }

        internal IBackgroundTransferService BackgroundTransferService
        {
            get {
                return this.backgroundTransferService ??
                       (this.backgroundTransferService = PhoneBackgroundTransferService.Instance);
            }

            set
            {
                Debug.Assert(value != null);
                this.backgroundTransferService = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Downloads the resource with the given path to the downloadLocation using the Windows Phone
        /// BackgroundTransferService.
        /// </summary>
        /// <param name="path">Path to the resource to download</param>
        /// <param name="downloadLocation">
        ///     The path to the file that will contain the download resource.
        ///     The downloadLocation must exist in /shared/transfers.
        /// </param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundDownloadAsync(
            string path,
            Uri downloadLocation)
        {
            return this.BackgroundDownloadAsync(path, downloadLocation, new CancellationToken(false),  null);
        }

        /// <summary>
        /// Downloads the resource with the given path to the downloadLocation using the Windows Phone
        /// BackgroundTransferService.
        /// </summary>
        /// <param name="path">Path to the resource to download</param>
        /// <param name="downloadLocation">
        ///     The path to the file that will contain the download resource.
        ///     The downloadLocation must exist in /shared/transfers.
        /// </param>
        /// <param name="ct">a token that is used to cancel the background download operation.</param>
        /// <param name="progress">an object that is called to report the background download's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public async Task<LiveOperationResult> BackgroundDownloadAsync(
            string path,
            Uri downloadLocation,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UrlInvalid"),
                                               "path");
                if (path == null)
                {
                    throw new ArgumentNullException("path", message);
                }

                throw new ArgumentException(message, "path");
            }

            if (downloadLocation == null)
            {
                throw new ArgumentNullException("downloadLocation");
            }

            string filename = Path.GetFileName(downloadLocation.OriginalString);
            if (string.IsNullOrEmpty(filename))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UriMissingFileName"),
                                               "downloadLocation");
                throw new ArgumentException(message, "downloadLocation");
            }

            if (!BackgroundTransferHelper.IsRootedInSharedTransfers(downloadLocation))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UriMustBeRootedInSharedTransfers"),
                                               "downloadLocation");
                throw new ArgumentException(message, "downloadLocation");
            }

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            Uri requestUri = this.GetResourceUri(path, ApiMethod.Download);

            var builder = new BackgroundDownloadOperation.Builder
            {
                RequestUri = requestUri,
                DownloadLocationOnDevice = downloadLocation,
                BackgroundTransferService = this.BackgroundTransferService,
                Progress = progress,
                BackgroundTransferPreferences = this.BackgroundTransferPreferences
            };

            if (!this.Session.IsValid)
            {
                LiveLoginResult result = await this.Session.AuthClient.RefreshTokenAsync();
                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    this.Session = result.Session;
                }
            }

            builder.AccessToken = this.Session.AccessToken;

            BackgroundDownloadOperation operation = builder.Build();

            ct.Register(operation.Cancel);

            return await operation.ExecuteAsync();
        }

        /// <summary>
        /// Uploads a file to the given path using the Windows Phone BackgroundTransferService.
        /// </summary>
        /// <param name="path">The path to the folder to upload the file to.</param>
        /// <param name="uploadLocation">The location of the file on the device to upload.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path,
            Uri uploadLocation,
            OverwriteOption option)
        {
            return this.BackgroundUploadAsync(path, uploadLocation, option, new CancellationToken(false), null);
        }

        /// <summary>
        /// Uploads a file to the given path using the Windows Phone BackgroundTransferService.
        /// </summary>
        /// <param name="path">The path to the folder to upload the file to.</param>
        /// <param name="uploadLocation">The location of the file on the device to upload.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <param name="ct">a token that is used to cancel the background upload operation.</param>
        /// <param name="progress">an object that is called to report the background upload's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> BackgroundUploadAsync(
            string path,
            Uri uploadLocation,
            OverwriteOption option,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (path == null)
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UrlInvalid"),
                                               "path");
                throw new ArgumentNullException("path", message);
            }

            if (uploadLocation == null)
            {
                throw new ArgumentNullException("uploadLocation");
            }

            string filename = Path.GetFileName(uploadLocation.OriginalString);
            if (string.IsNullOrEmpty(filename))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UriMissingFileName"),
                                               "uploadLocation");
                throw new ArgumentException(message, "uploadLocation");
            }

            if (!BackgroundTransferHelper.IsRootedInSharedTransfers(uploadLocation))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UriMustBeRootedInSharedTransfers"),
                                               "uploadLocation");
                throw new ArgumentException(message, "uploadLocation");
            }

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var builder = new BackgroundUploadOperation.Builder
            {
                BackgroundTransferService = this.BackgroundTransferService,
                Client = this,
                Path = path,
                UploadLocationOnDevice = uploadLocation,
                OverwriteOption = option,
                Progress = progress,
                BackgroundTransferPreferences = this.BackgroundTransferPreferences
            };

            BackgroundUploadOperation operation = builder.Build();

            ct.Register(operation.Cancel);

            return operation.ExecuteAsync();
        }

        /// <summary>
        /// Retrieves all the pending background downloads created by the Live SDK.
        /// Useful for when the application comes back from a tombstoned or out of memory state.
        /// </summary>
        /// <returns>All the pending background downloads created by the Live SDK.</returns>
        public IEnumerable<LivePendingDownload> GetPendingBackgroundDownloads()
        {
            var requests = new List<BackgroundTransferRequest>();
            this.BackgroundTransferService.FindAllLiveSdkRequests(requests);

            var downloads = new List<LivePendingDownload>();
            foreach (BackgroundTransferRequest request in requests)
            {
                if (BackgroundTransferHelper.IsDownloadRequest(request))
                {
                    downloads.Add(new LivePendingDownload(this.BackgroundTransferService, request));
                }
            }

            return downloads;
        }

        /// <summary>
        /// Retrieves all the pending background uploads created by the Live SDK.
        /// Useful for when the application comes back from a tombstoned or out of memory state.
        /// </summary>
        /// <returns>All the pending background uploads created by the Live SDK.</returns>
        public IEnumerable<LivePendingUpload> GetPendingBackgroundUploads()
        {
            var requests = new List<BackgroundTransferRequest>();
            this.BackgroundTransferService.FindAllLiveSdkRequests(requests);
            var uploads = new List<LivePendingUpload>();

            foreach (BackgroundTransferRequest request in requests)
            {
                if (BackgroundTransferHelper.IsUploadRequest(request))
                {
                    uploads.Add(new LivePendingUpload(this.backgroundTransferService, request));
                }
            }

            return uploads;
        }

        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> DownloadAsync(string path)
        {
            return this.DownloadAsync(path, new CancellationToken(false), null);
        }

        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="ct">a token that is used to cancel the download operation.</param>
        /// <param name="progress">an object that is called to report the download's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveDownloadOperationResult> DownloadAsync(
            string path, 
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UrlInvalid"),
                                               "path");
                throw new ArgumentException(message, "path");
            }

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var tcs = new TaskCompletionSource<LiveDownloadOperationResult>();
            var operation = new DownloadOperation(
                this,
                this.GetResourceUri(path, ApiMethod.Download),
                progress != null ? new Action<LiveOperationProgress>(progress.Report) : null,
                SynchronizationContextWrapper.Current)
            {
                OperationCompletedCallback = (LiveDownloadOperationResult result) =>
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
                }
            };

            ct.Register(operation.Cancel);

            operation.Execute();

            return tcs.Task;
        }

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the file content.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> UploadAsync(
            string path,
            string fileName,
            Stream inputStream,
            OverwriteOption option)
        {
            return this.UploadAsync(
                path, 
                fileName, 
                inputStream, 
                option,
                new CancellationToken(false), 
                null);
        }

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the file content.</param>
        /// <param name="option">an enum to specify the overwrite behavior if a file with the same name already exists.</param>
        /// <param name="ct">a token that is used to cancel the upload operation.</param>
        /// <param name="progress">an object that is called to report the upload's progress.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> UploadAsync(
            string path,
            string fileName,
            Stream inputStream,
            OverwriteOption option,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("UrlInvalid"),
                                               "path");
                throw new ArgumentException(message, "path");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("InvalidNullOrEmptyParameter"),
                                               "fileName");
                throw new ArgumentException(message, "fileName");
            }

            if (inputStream == null)
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("InvalidNullParameter"),
                                               "inputStream");
                throw new ArgumentNullException("inputStream", message);
            }

            if (!inputStream.CanRead)
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("StreamNotReadable"),
                                               "inputStream");
                throw new ArgumentException(message, "inputStream");
            }

            var operation = new UploadOperation(
                this,
                this.GetResourceUri(path, ApiMethod.Upload),
                fileName,
                inputStream,
                option,
                progress != null ? new Action<LiveOperationProgress>(progress.Report) : null,
                SynchronizationContextWrapper.Current);

            return this.ExecuteApiOperation(operation, ct);
        }

        #endregion
    }
}
