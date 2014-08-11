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
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;

    internal abstract class WebOperation : Operation
    {
        #region Constructors

        protected WebOperation(Uri url, string body, SynchronizationContextWrapper syncContext)
            : base(syncContext)
        {
            Debug.Assert(url != null, "url must not be null.");

            this.Url = url;
            this.Body = body;
        }

        #endregion

        #region Properties

        public string Body { get; internal set; }

        public WebRequest Request { get; internal set; }

        public Uri Url { get; internal set; }

        #endregion

        #region Public Methods

        public override void Cancel()
        {
            if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
            {
                // no-op
                return;
            }

            this.Status = OperationStatus.Cancelled;

            if (this.Request != null)
            {
                this.Request.Abort();
            }
            else
            {
                this.OnCancel();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Callback used for WebRequest.BeginGetRequestStream.
        /// </summary>
        protected virtual void OnGetRequestStreamCompleted(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                try
                {
                    using (Stream requestStream = this.Request.EndGetRequestStream(ar))
                    {
                        if (!string.IsNullOrEmpty(this.Body))
                        {
                            byte[] dataInBytes = Encoding.UTF8.GetBytes(this.Body);
                            requestStream.Write(dataInBytes, 0, dataInBytes.Length);
                        }
                    }

                    this.Request.BeginGetResponse(this.OnGetResponseCompleted, null);
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        this.OnWebResponseReceived(exception.Response);
                    }
                }
                catch (IOException)
                {
                    this.OnWebResponseReceived(null);
                }
            }
        }

        /// <summary>
        /// Callback used for WebRequest.BeginGetResponse.
        /// </summary>
        protected void OnGetResponseCompleted(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                try
                {
                    this.OnWebResponseReceived(this.Request.EndGetResponse(ar));
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        using (exception.Response)
                        {
                            this.OnWebResponseReceived(exception.Response);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Abstract method that is called when a WebResponse was received from the server.
        /// The response could be successful or contain an error or null on an exception.
        /// </summary>
        /// <param name="response">The WebResponse from the server or null if their was an IOException.</param>
        protected abstract void OnWebResponseReceived(WebResponse response);

        #endregion
    }
}