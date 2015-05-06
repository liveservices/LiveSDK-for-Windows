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
    /// This class implements the upload operation on Windows 8 using the background uploader.
    /// </summary>
    internal class TailoredUploadOperation : ApiOperation
    {
        #region Instance member variables

        private const uint MaxUploadResponseLength = 10240;
        private BT.UploadOperation uploadOp;

        /// <summary>
        /// CancellationTokenSource used when calling the BT.UploadOperation.
        /// </summary>
        private CancellationTokenSource uploadOpCts;

        private bool isAttach;

        #endregion

        #region Cosntructors

        /// <summary>
        /// This class implements the upload operation on Windows 8 using the background uploader.
        /// </summary>
        /// <remarks>This constructor is used when uploading a file from the file system.</remarks>
        public TailoredUploadOperation(
            LiveConnectClient client, 
            Uri url, 
            string fileName,
            IStorageFile inputFile,
            OverwriteOption option,
            IProgress<LiveOperationProgress> progress,
            SynchronizationContextWrapper syncContext)
            : this(client, url, fileName, option, progress, syncContext)
        {
            Debug.Assert(inputFile != null, "inputFile is null.");
            this.InputFile = inputFile;
        }

        /// <summary>
        /// This class implements the upload operation on Windows 8 using the background uploader.
        /// </summary>
        /// <remarks>This constructor is used when uploading a stream created by the application.</remarks>
        public TailoredUploadOperation(
            LiveConnectClient client,
            Uri url,
            string fileName,
            IInputStream inputStream,
            OverwriteOption option,
            IProgress<LiveOperationProgress> progress,
            SynchronizationContextWrapper syncContext)
            : this(client, url, fileName, option, progress, syncContext)
        {
            Debug.Assert(inputStream != null, "inputStream is null.");
            this.InputStream = inputStream;
        }

        /// <summary>
        /// This class implements the upload operation on Windows 8 using the background uploader.
        /// </summary>
        /// <remarks>This constructor is used when uploading a stream created by the application.</remarks>
        internal TailoredUploadOperation(
            LiveConnectClient client,
            Uri url,
            string fileName,
            OverwriteOption option,
            IProgress<LiveOperationProgress> progress,
            SynchronizationContextWrapper syncContext)
            : base(client, url, ApiMethod.Upload, null, syncContext)
        {
            this.FileName = fileName;
            this.Progress = progress;
            this.OverwriteOption = option;
        }

        /// <summary>
        /// Creates a new TailoredDownloadOperation instance.
        /// </summary>
        /// <remarks>This constructor should only be used when attaching to an pending download.</remarks>
        internal TailoredUploadOperation()
            : base(null, null, ApiMethod.Upload, null, null)
        {
        }

        #endregion

        #region Properties

        public string FileName { get; private set; }

        public OverwriteOption OverwriteOption { get; private set; }

        public IStorageFile InputFile { get; private set; }

        public IInputStream InputStream { get; private set; }

        public IProgress<LiveOperationProgress> Progress { get; private set; }

        #endregion

        #region Public methods

        public async void Attach(BT.UploadOperation uploadOp)
        {
            Debug.Assert(uploadOp != null);

            this.uploadOp = uploadOp;
            this.isAttach = true;
            this.uploadOpCts = new CancellationTokenSource();
            var progressHandler = new Progress<BT.UploadOperation>(this.OnUploadProgress);

            try
            {
                // Since we don't provide API for apps to attach to pending upload operations, we have no
                // way to invoke the app event handler to provide any feedback. We would just ignore the result here.
                this.uploadOp = await this.uploadOp.AttachAsync().AsTask(this.uploadOpCts.Token, progressHandler);
            }
            catch
            {
                // Ignore errors as well. 
            }
        }

        public override void Cancel()
        {
            if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
            {
                return;
            }

            this.Status = OperationStatus.Cancelled;

            if (this.uploadOp != null)
            {
                this.uploadOpCts.Cancel();
            }
            else
            {
                base.OnCancel();
            }
        }

        #endregion

        #region Protected methods

        protected override void OnExecute()
        {
            var getUploadLinkOp = new GetUploadLinkOperation(
                    this.LiveClient,
                    this.Url,
                    this.FileName,
                    this.OverwriteOption,
                    null);

            getUploadLinkOp.OperationCompletedCallback = this.OnGetUploadLinkCompleted;
            getUploadLinkOp.Execute();
        }

        #endregion

        #region Private methods

        private async void OnGetUploadLinkCompleted(LiveOperationResult result)
        {
            if (this.Status == OperationStatus.Cancelled)
            {
                base.OnCancel();
                return;
            }

            if (result.Error != null || result.IsCancelled)
            {
                this.OnOperationCompleted(result);
                return;
            }

            var uploadUrl = new Uri(result.RawResult, UriKind.Absolute);

            // NOTE: the GetUploadLinkOperation will return a uri with the overwite, suppress_response_codes,
            // and suppress_redirect query parameters set.
            Debug.Assert(uploadUrl.Query != null);
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.Overwrite));
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.SuppressRedirects));
            Debug.Assert(uploadUrl.Query.Contains(QueryParameters.SuppressResponseCodes));

            var uploader = new BT.BackgroundUploader();
            uploader.Group = LiveConnectClient.LiveSDKUploadGroup;
            if (this.LiveClient.Session != null)
            {
                uploader.SetRequestHeader(
                    ApiOperation.AuthorizationHeader,
                    AuthConstants.BearerTokenType + " " + this.LiveClient.Session.AccessToken);
            }
            uploader.SetRequestHeader(ApiOperation.LibraryHeader, Platform.GetLibraryHeaderValue());
            uploader.Method = HttpMethods.Put;

            this.uploadOpCts = new CancellationTokenSource();
            Exception webError = null;

            LiveOperationResult opResult = null;
            try
            {
                if (this.InputStream != null)
                {
                    this.uploadOp = await uploader.CreateUploadFromStreamAsync(uploadUrl, this.InputStream);
                }
                else
                {
                    this.uploadOp = uploader.CreateUpload(uploadUrl, this.InputFile);
                }

                var progressHandler = new Progress<BT.UploadOperation>(this.OnUploadProgress);

                this.uploadOp = await this.uploadOp.StartAsync().AsTask(this.uploadOpCts.Token, progressHandler);
            }
            catch (TaskCanceledException exception)
            {
                opResult = new LiveOperationResult(exception, true);
            }
            catch (Exception exp)
            {
                // This might be an server error. We will read the response to determine the error message.
                webError = exp;
            }

            if (opResult == null)
            {
                try
                {
                    IInputStream responseStream = this.uploadOp == null ? null : this.uploadOp.GetResultStreamAt(0);
                    if (responseStream == null)
                    {
                        var error = new LiveConnectException(
                            ApiOperation.ApiClientErrorCode,
                            ResourceHelper.GetString("ConnectionError"));
                        opResult = new LiveOperationResult(error, false);
                    }
                    else
                    {
                        var reader = new DataReader(responseStream);
                        uint length = await reader.LoadAsync(MaxUploadResponseLength);
                        opResult = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Upload);

                        if (webError != null && opResult.Error != null && !(opResult.Error is LiveConnectException))
                        {
                            // If the error did not come from the api service,
                            // we'll just return the error thrown by the uploader.
                            opResult = new LiveOperationResult(webError, false);
                        }
                    }
                }
                catch (COMException exp)
                {
                    opResult = new LiveOperationResult(exp, false);
                }
                catch (FileNotFoundException exp)
                {
                    opResult = new LiveOperationResult(exp, false);
                }
            }

            this.OnOperationCompleted(opResult);
        }

        /// <summary>
        /// Called when a progress event is raised from the BT.UploadOperation and this
        /// will call this TailoredUploadOperation's progress handler.
        /// </summary>
        private void OnUploadProgress(BT.UploadOperation uploadOp)
        {
            Debug.Assert(uploadOp != null);

            if (uploadOp.Progress.Status == BT.BackgroundTransferStatus.Error ||
                uploadOp.Progress.Status == BT.BackgroundTransferStatus.Canceled
                || this.isAttach
                || this.Progress == null)
            {
                return;
            }

            this.Progress.Report(
                new LiveOperationProgress(
                    (long)uploadOp.Progress.BytesSent,
                    (long)uploadOp.Progress.TotalBytesToSend));
        }

        #endregion
    }
}
