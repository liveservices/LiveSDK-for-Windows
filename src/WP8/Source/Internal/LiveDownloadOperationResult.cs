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
