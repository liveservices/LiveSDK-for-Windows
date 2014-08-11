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

    internal class StreamCopyOperation : Operation
    {
        #region Class member variables

        /// <summary>
        /// Size of buffer allocated for stream copying.
        /// </summary>
        internal static readonly int BufferSize = 102400;

        #endregion

        #region Instance member variables

        private readonly byte[] buffer;
        private long totalBytesCopied;
        private readonly Action<bool, Exception> copyCompletedCallback;
        private readonly object progress;

        #endregion

        #region Constructors

        public StreamCopyOperation(
            LiveConnectClient client, 
            ApiMethod method, 
            Stream inputStream, 
            Stream outputStream, 
            long contentLength, 
            object progress,
            SynchronizationContextWrapper syncContext, 
            Action<bool, Exception> onCopyCompleted)
            : base(syncContext)
        {
            Debug.Assert(client != null, "client must not be null.");
            Debug.Assert(
                method == ApiMethod.Download || method == ApiMethod.Upload,
                "Only Download and Upload methods are allowed.");
            Debug.Assert(inputStream.CanRead, "Input stream is not readable.");
            Debug.Assert(outputStream.CanWrite, "Output stream is not writable.");

            this.LiveClient = client;
            this.Method = method;
            this.InputStream = inputStream;
            this.OutputStream = outputStream;
            this.ContentLength = contentLength;
            this.buffer = new byte[StreamCopyOperation.BufferSize];
            this.copyCompletedCallback = onCopyCompleted;
            this.progress = progress;
        }

        #endregion

        #region Properties

        public LiveConnectClient LiveClient { get; private set; }

        public ApiMethod Method { get; private set; }

        public Stream InputStream { get; private set; }

        public Stream OutputStream { get; private set; }

        public long ContentLength { get; private set; }

        #endregion

        #region Protected methods

        protected override void OnExecute()
        {
            this.ReadNextChunk();
        }

        protected override void OnCancel()
        {
            this.OnOperationCompleted(null);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Calls the copyCompletedCallback delegate.
        /// This method should be called when the operation is completed.
        /// </summary>
        /// <param name="error">Any exception that happened or null</param>
        private void OnOperationCompleted(Exception error)
        {
            if (this.copyCompletedCallback != null)
            {
                this.copyCompletedCallback(this.IsCancelled, error);
            }
        }

        private void OnReadInputComplete(int bytesRead, Exception error)
        {
            if (error != null)                
            {
                this.OnOperationCompleted(error);
                return;
            }

            if (this.IsCancelled)
            {
                this.OnOperationCompleted(null);
                return;
            }

            if (bytesRead > 0)
            {
                Platform.StreamWriteAsync(
                    this.OutputStream, 
                    this.buffer, 
                    0, 
                    bytesRead, 
                    this.Dispatcher, 
                    this.OnWriteOutputComplete);
            }
            else
            {
                // Reached the end of the stream, all done.
                if (this.totalBytesCopied != this.ContentLength)
                {
                    // The content length may not be accurate, make sure our progress event reaches the end.
                    this.ContentLength = this.totalBytesCopied;
                    this.RaiseProgressChangedEvent();
                }

                this.OnOperationCompleted(null);
            }
        }

        private void OnWriteOutputComplete(int bytesWritten, Exception error)
        {
            if (error != null)
            {
                this.OnOperationCompleted(error);
                return;
            }

            this.totalBytesCopied += bytesWritten;

            this.RaiseProgressChangedEvent();

            // Continues to read
            this.ReadNextChunk();
        }

        private void RaiseProgressChangedEvent()
        {
            Platform.ReportProgress(
                this.progress, 
                new LiveOperationProgress(this.totalBytesCopied, this.ContentLength));
        }

        private void ReadNextChunk()
        {
            Exception error = null;

            try
            {
                if (this.IsCancelled)
                {
                    this.OnOperationCompleted(null);
                }
                else
                {
                    Platform.StreamReadAsync(
                        this.InputStream, 
                        this.buffer, 
                        0, 
                        StreamCopyOperation.BufferSize, 
                        this.Dispatcher, 
                        this.OnReadInputComplete);
                }
            }
            catch (IOException ioe)
            {
                error = ioe;
            }
            catch (NotSupportedException nse)
            {
                error = nse;
            }

            if (error != null)
            {
                this.OnOperationCompleted(error);
            }
        }

        #endregion
    }
}