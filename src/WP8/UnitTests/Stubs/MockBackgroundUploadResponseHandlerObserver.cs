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
