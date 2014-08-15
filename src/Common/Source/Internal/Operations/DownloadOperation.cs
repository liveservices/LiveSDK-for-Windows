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
    using System.Globalization;
    using System.IO;
    using System.Net;

    internal class DownloadOperation : ApiOperation
    {
        #region Class member variables

        private const string HttpHeaderContentLength = "Content-Length";
        private const int UnknownFileSize = 0;

        #endregion

        #region Instance member variables

        private WebResponse response;
        private Stream outputStream;
        private readonly object progress;
        private StreamCopyOperation streamCopyOperation;

        #endregion

        #region Constructors

        public DownloadOperation(
            LiveConnectClient client, 
            Uri url, 
            object progress, 
            SynchronizationContextWrapper syncContext)
            : base(client, url, ApiMethod.Download, null, syncContext)
        {
            this.progress = progress;
        }

        #endregion

        #region Properties

        public new Action<LiveDownloadOperationResult> OperationCompletedCallback { get; set; }

        #endregion

        #region Protected methods

        protected override void OnCancel()
        {
            if (this.streamCopyOperation != null)
            {
                // this.streamCopyOperation's completed callback will call our CompleteOperation
                this.streamCopyOperation.Cancel();
            }
            else
            {
                // cancelled when the operation has not started yet.
                this.CompleteOperation(null);
            }
        }

        protected override void OnExecute()
        {
            if (this.Url.OriginalString.StartsWith(this.LiveClient.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                if (!this.PrepareRequest())
                {
                    return;
                }
            }
            else
            {
                this.Request = WebRequestFactory.Current.CreateWebRequest(this.Url, HttpMethods.Get);
            }

            Platform.SetDownloadRequestOption(this.Request as HttpWebRequest);
            this.Request.BeginGetResponse(this.OnGetResponseCompleted, null);
        }

        protected override void OnWebResponseReceived(WebResponse response)
        {
            // We are done with the HttpWebRequest so let's remove our reference to it.
            // If we hold on to it and Cancel is called, our parent class, WebOperation, will call this.Request.Abort
            // and this causes a Deadlock in the stream.BeginRead call in StreamCopyOperation. Not fun.
            this.Request = null;

            HttpStatusCode status = ((HttpWebResponse)response).StatusCode;
            if (status != HttpStatusCode.OK)
            {
                var result = this.CreateOperationResultFrom(response);
                if (result.Error is FormatException)
                {
                    // We do expect non-JSON errors from other data providers for download requests.
                    // If we can't understand the response body, we'll just return a generic error message.
                    var error = new LiveConnectException(
                        ApiOperation.ApiServerErrorCode,
                        string.Format(
                            CultureInfo.CurrentUICulture,
                            ResourceHelper.GetString("ServerErrorWithStatus"),
                            status.ToString()));

                    result = new LiveOperationResult(error, false);
                }

                this.CompleteOperation(result.Error);
            }
            else if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
            {
                var result = this.CreateOperationResultFrom(response);
                this.CompleteOperation(result.Error);
            }
            else
            {
                this.response = response;
                Stream responseStream = this.response.GetResponseStream();
                this.outputStream = new MemoryStream();
                long totalBytesToReceive;
                string contentLength = response.Headers[DownloadOperation.HttpHeaderContentLength];
                if (!string.IsNullOrEmpty(contentLength))
                {
                    if (!long.TryParse(contentLength, out totalBytesToReceive))
                    {
                        totalBytesToReceive = DownloadOperation.UnknownFileSize;
                    }
                }
                else
                {
                    totalBytesToReceive = DownloadOperation.UnknownFileSize;
                }
                
                this.streamCopyOperation = new StreamCopyOperation(
                        this.LiveClient,
                        ApiMethod.Download,
                        responseStream,
                        this.outputStream,
                        totalBytesToReceive,
                        this.progress,
                        this.Dispatcher,
                        (isCancelled, error) =>
                        {
                            if (isCancelled)
                            {
                                this.Cancel();
                            }

                            this.CompleteOperation(error);
                        });

                Platform.RegisterForCancel(null, this.streamCopyOperation.Cancel);

                this.streamCopyOperation.Execute();
            }
        }

        #endregion

        #region Private methods

        private void CompleteOperation(Exception error)
        {
            LiveDownloadOperationResult opResult;

            if (this.response != null)
            {
                this.response.Close();
            }

            if (this.IsCancelled)
            {
                opResult = new LiveDownloadOperationResult(null, true);
            }
            else if (error != null)
            {
                opResult = new LiveDownloadOperationResult(error, false);
            }
            else
            {
                this.outputStream.Seek(0, 0);
                opResult = new LiveDownloadOperationResult(this.outputStream);
            }

            Action<LiveDownloadOperationResult> callback = this.OperationCompletedCallback;
            if (callback != null)
            {
                callback(opResult);
            }
        }

        #endregion
    }
}
