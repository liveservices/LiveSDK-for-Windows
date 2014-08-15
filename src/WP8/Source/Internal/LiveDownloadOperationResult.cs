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
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// This class respresent the result of a download operation.
    /// </summary>
    public class LiveDownloadOperationResult
    {
        #region Constructors

        /// <summary>
        /// Constructs a LiveDownloadOperationResult object.
        /// </summary>
        /// <param name="stream">The IInputStream object for the downloaded file.</param>
        internal LiveDownloadOperationResult(Stream stream)
        {
            this.Stream = stream;
        }

        /// <summary>
        /// Constructs a new LiveOperationResult object when an error happens.
        /// </summary>
        /// <param name="error">The error object.</param>
        /// <param name="cancelled">True if the operation was cancelled.</param>
        internal LiveDownloadOperationResult(Exception error, bool cancelled)
        {
            Debug.Assert(error != null || cancelled);

            this.Error = error;
            this.IsCancelled = cancelled;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the stream object for the downloaded content.
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Gets the error object.
        /// </summary>
        internal Exception Error { get; private set; }

        /// <summary>
        /// Gets whether the operation was cancelled.
        /// </summary>
        internal bool IsCancelled { get; private set; }

        #endregion
    }
}
