// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
