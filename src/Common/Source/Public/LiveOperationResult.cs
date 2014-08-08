namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// This class contains the result of an api operation.
    /// </summary>
    public class LiveOperationResult
    {
        #region Constructors

        /// <summary>
        /// Constructs a new LiveOperationResult object.
        /// </summary>
        /// <param name="result">The JSON result parsed into a dictionary object.</param>
        /// <param name="rawResult">The raw JSON result in text format.</param>
        internal LiveOperationResult(IDictionary<string, object> result, string rawResult)
        {
            this.Result = result ?? new DynamicDictionary();

            this.RawResult = rawResult;
        }

        /// <summary>
        /// Constructs a new LiveOperationResult object when an error happens.
        /// </summary>
        /// <param name="error">The error object.</param>
        /// <param name="cancelled">True if the operation was cancelled.</param>
        internal LiveOperationResult(Exception error, bool cancelled)
        {
            Debug.Assert(error != null || cancelled);

            this.Error = error;
            this.IsCancelled = cancelled;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the JSON result in dictionary format.
        /// </summary>
        public IDictionary<string, object> Result { get; private set; }

        /// <summary>
        /// Gets the JSON result in text format.
        /// </summary>
        public string RawResult { get; private set; }

        /// <summary>
        /// Gets the error object.
        /// </summary>
        internal Exception Error { get; private set; }

        /// <summary>
        /// Gets whether the operation was cancelled.
        /// </summary>
        internal bool IsCancelled { get; private set; }

        #endregion

        #region Methods

        internal static LiveOperationResult FromResponse(string response)
        {
            var creator = new Creator();
            ServerResponseReader reader = ServerResponseReader.Instance;
            reader.Read(response, creator);

            return creator.Result;
        }

        #endregion

        #region Internal Classes

        private class Creator : IServerResponseReaderObserver
        {
            private LiveOperationResult result;

            public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
            {
                this.Result = new LiveOperationResult(result, rawResult);
            }

            public void OnErrorResponse(string code, string message)
            {
                var error = new LiveConnectException(code, message);
                this.Result = new LiveOperationResult(error, false);
            }

            public void OnInvalidJsonResponse(FormatException exception)
            {
                this.Result = new LiveOperationResult(exception, false);
            }

            public LiveOperationResult Result
            {
                get
                {
                    Debug.Assert(this.result != null);
                    return this.result;
                }
                private set
                {
                    Debug.Assert(value != null);
                    this.result = value;
                }
            }
        }

        #endregion
    }
}
