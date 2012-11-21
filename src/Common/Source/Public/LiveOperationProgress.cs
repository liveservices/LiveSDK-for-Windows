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
