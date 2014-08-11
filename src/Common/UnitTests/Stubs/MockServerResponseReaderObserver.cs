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
