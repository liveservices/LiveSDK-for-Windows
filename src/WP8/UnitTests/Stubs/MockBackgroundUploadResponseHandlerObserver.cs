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
    using Microsoft.Live.Phone;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockBackgroundUploadResponseHandlerObserver : IBackgroundUploadResponseHandlerObserver
    {
        private readonly IDictionary<string, object> expectedResult;
        private readonly string expectedRawResult;
        private readonly Exception expectedException;
        private readonly string expectedCode;
        private readonly string expectedMessage;

        private IDictionary<string, object> actualResult;
        private string actualRawResult;
        private Exception actualException;
        private string actualCode;
        private string actualMessage;

        public MockBackgroundUploadResponseHandlerObserver(
            IDictionary<string, object> expectedResult,
            string expectedRawResult)
        {
            this.expectedResult = expectedResult;
            this.expectedRawResult = expectedRawResult;
        }

        public MockBackgroundUploadResponseHandlerObserver(Exception expectedException)
        {
            this.expectedException = expectedException;
        }

        public MockBackgroundUploadResponseHandlerObserver(
            string expectedCode,
            string expectedMessage)
        {
            this.expectedCode = expectedCode;
            this.expectedMessage = expectedMessage;
        }

        public IDictionary<string, object> ActualResult
        {
            get { return this.actualResult; }
        }

        public string ActualRawResult
        {
            get { return this.actualRawResult; }
        }

        public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
        {
            this.actualResult = result;
            this.actualRawResult = rawResult;
        }

        public void OnErrorResponse(string code, string message)
        {
            this.actualCode = code;
            this.actualMessage = message;
        }

        public void OnError(Exception exception)
        {
            this.actualException = exception;
        }

        public void CheckOnSuccessCalledCorrectly()
        {
            Assert.AreEqual(
                this.expectedRawResult, 
                this.actualRawResult,
                "OnSuccessResponse was called with the incorrect rawResult.");
        }

        public void CheckOnErrorCalledCorrectly()
        {
            if (this.expectedCode != null && this.expectedMessage != null)
            {
                Assert.AreEqual(
                    this.expectedCode,
                    this.actualCode,
                    "OnErrorResponse was called with the incorrect code.");
                Assert.AreEqual(
                    this.expectedMessage,
                    this.actualMessage,
                    "OnErrorResponse was called with the incorrect message.");
            }
            else
            {
                Assert.AreEqual(
                    this.expectedException, 
                    this.actualException,
                    "OnErrorResponse was called with the incorrect exception.");
            }
        }
    }
}
