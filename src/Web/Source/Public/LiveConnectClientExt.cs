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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Live.Operations;

    /// <summary>
    /// This is the class that applications use to interact with the Api service.
    /// </summary>
    public sealed partial class LiveConnectClient
    {
        #region Public Methods

        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        public Task<LiveDownloadOperationResult> DownloadAsync(string path)
        {
            return this.DownloadAsync(path, new CancellationToken(false), null);
        }

        /// <summary>
        /// Download a file into a stream.
        /// </summary>
        /// <param name="path">relative or absolute uri to the file to be downloaded.</param>
        /// <param name="ct">a cancellation token</param>
        /// <param name="progress">a progress event callback handler</param>
        public Task<LiveDownloadOperationResult> DownloadAsync(
            string path,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(path, "path");

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var tcs = new TaskCompletionSource<LiveDownloadOperationResult>();
            var op = new DownloadOperation(
                this,
                this.GetResourceUri(path, ApiMethod.Download),
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

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the file content.</param>
        /// <param name="option">
        ///     a enum to specify the overwrite behavior if a file with the same name already exists.  
        ///     Default is DoNotOverwrite.
        /// </param>
        public Task<LiveOperationResult> UploadAsync(
            string path,
            string fileName,
            Stream inputStream,
            OverwriteOption option)
        {
            return this.UploadAsync(path, fileName, inputStream, option, new CancellationToken(false), null);
        }

        /// <summary>
        /// Upload a file to the server.
        /// </summary>
        /// <param name="path">relative or absolute uri to the location where the file should be uploaded to.</param>
        /// <param name="fileName">name for the uploaded file.</param>
        /// <param name="inputStream">Stream that contains the file content.</param>
        /// <param name="option">
        ///     a enum to specify the overwrite behavior if a file with the same name already exists.  
        ///     Default is DoNotOverwrite.
        /// </param>
        /// <param name="ct">a cancellation token</param>
        /// <param name="progress">a progress event callback handler</param>
        public Task<LiveOperationResult> UploadAsync(
            string path,
            string fileName,
            Stream inputStream,
            OverwriteOption option,
            CancellationToken ct,
            IProgress<LiveOperationProgress> progress)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(path, "path");
            LiveUtility.ValidateNotNullParameter(inputStream, "inputStream");

            if (string.IsNullOrEmpty(fileName))
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("InvalidNullOrEmptyParameter"),
                                               "fileName");
                throw new ArgumentException(message, "fileName");
            }

            if (!inputStream.CanRead)
            {
                string message = String.Format(CultureInfo.CurrentUICulture,
                                               ResourceHelper.GetString("StreamNotReadable"),
                                               "inputStream");
                throw new ArgumentException(message, "inputStream");
            }

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var tcs = new TaskCompletionSource<LiveOperationResult>();
            var op = new UploadOperation(
                this,
                this.GetResourceUri(path, ApiMethod.Upload),
                fileName,
                inputStream,
                option,
                progress,
                null);

            op.OperationCompletedCallback = (LiveOperationResult result) =>
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
