namespace Microsoft.Live
{
    using System;

    public class LiveConnectException : Exception
    {
        public LiveConnectException()
        {
        }

        public LiveConnectException(string errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public LiveConnectException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        public string ErrorCode { get; private set; }

        public override string ToString()
        {
            return this.ErrorCode + ": " + base.ToString();
        }
    }
}
