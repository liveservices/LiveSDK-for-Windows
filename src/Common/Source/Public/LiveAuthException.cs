namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// An exception type used by the auth client.
    /// </summary>
    public class LiveAuthException : Exception
    {
        /// <summary>
        /// Constructs a new exception instance.
        /// </summary>
        public LiveAuthException()
        {
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// </summary>
        public LiveAuthException(string errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// <param name="innerException">The wrapped exception.</param>
        /// </summary>
        public LiveAuthException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// <param name="requestState">The request state that is encoded in the login Url parameter.</param>
        /// </summary>
        internal LiveAuthException(string errorCode, string message, string requestState)
            : base(message)
        {
            this.ErrorCode = errorCode;
            this.State = requestState;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Gets the request state value that is encoded in the login Url parameter.
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Customize the display string for the exception object.
        /// </summary>
        public override string ToString()
        {
            return this.ErrorCode + ": " + base.ToString();
        }
    }
}
