namespace Microsoft.Live.UnitTest.Stubs
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Live;

    public class MockServerResponseReaderObserver : IServerResponseReaderObserver
    {
        private bool onSuccessResponseCalled;
        private bool onErrorResponseCalled;
        private bool onInvalidJsonResponseCalled;

        public string RawResult { get; private set; }

        public IDictionary<string, object> Result { get; private set; }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public FormatException Exception { get; private set; }

        public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
        {
            this.onSuccessResponseCalled = true;
            this.Result = result;
            this.RawResult = rawResult;
        }

        public void OnErrorResponse(string code, string message)
        {
            this.onErrorResponseCalled = true;
            this.Code = code;
            this.Message = message;
        }

        public void OnInvalidJsonResponse(FormatException exception)
        {
            this.onInvalidJsonResponseCalled = true;
            this.Exception = exception;
        }

        public bool HasCalledOnSuccessResponse()
        {
            return this.onSuccessResponseCalled;
        }

        public bool HasCalledOnErrorResponse()
        {
            return this.onErrorResponseCalled;
        }

        public bool HasCalledOnInvalidJsonResponse()
        {
            return this.onInvalidJsonResponseCalled;
        }
    }
}
