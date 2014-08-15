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

    /// <summary>
    /// This class contains progress data for upload/download operations.
    /// </summary>
    public class LiveOperationProgress
    {
        #region Constructors

        /// <summary>
        /// Constructor to create a LiveOperationProgress object.
        /// </summary>
        /// <param name="bytesTransferred">Number of bytes transferred.</param>
        /// <param name="totalBytes">Number of total bytes to be transferred.</param>
        internal LiveOperationProgress(long bytesTransferred, long totalBytes)
        {
            Debug.Assert(bytesTransferred >= 0);
            Debug.Assert(totalBytes >= 0);

            this.BytesTransferred = bytesTransferred;
            this.TotalBytes = totalBytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of bytes transferred. 
        /// </summary>
        public long BytesTransferred { get; private set; }

        /// <summary>
        /// Gets the number of total bytes to be transferred.
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        /// Gets the percentage completed.
        /// </summary>
        /// <remarks>If the total bytes is 0 (unknown), we will always return 0%.</remarks>
        public double ProgressPercentage
        {
            get
            {
                return (this.TotalBytes == 0) ? 0 : ((double)this.BytesTransferred) / this.TotalBytes * 100;
            }
        }

        #endregion
    }
}
