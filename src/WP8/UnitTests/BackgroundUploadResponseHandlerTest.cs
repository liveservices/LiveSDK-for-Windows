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

namespace Microsoft.Live.WP8.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;

    using Microsoft.Live.Phone;
    using Microsoft.Live.UnitTest.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundUploadResponseHandlerTest
    {
        [TestMethod]
        public void TestReadAndDeleteFile()
        {
            string fileName = "my_file.txt";
            var expectedResult = new Dictionary<string, object>();
            expectedResult["foo"] = "bar";
            string fileContents = @"{""foo"": ""bar""}";
            CreateFile(fileName, fileContents);

            var downloadLocation = new Uri(fileName, UriKind.RelativeOrAbsolute);
            var observer = new MockBackgroundUploadResponseHandlerObserver(expectedResult, fileContents);

            var responseHandler =
                new BackgroundUploadResponseHandler(downloadLocation, observer);

            Action result = responseHandler.OnDoWork();
            result.Invoke();

            IDictionary<string, object> actualResult = observer.ActualResult;
            Assert.AreEqual(expectedResult["foo"], actualResult["foo"], "Did not receive the correct result.");

            Assert.AreEqual(fileContents, observer.ActualRawResult, "Did not receive the correct raw result");

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Assert.IsFalse(store.FileExists(fileName), "The created file was not deleted");
            }
        }

        [TestMethod]
        public void TestReadAndDeleteFileWithError()
        {
            string fileName = "my_file.txt";
            var error = new Dictionary<string,object>();
            error["code"] = "the code";
            error["message"] = "the message";

            var expectedResult = new Dictionary<string,object>();
            expectedResult["error"] = error;
            string fileContents = @"{""error"": {""code"": ""the code"", ""message"": ""the message""} }";
            CreateFile(fileName, fileContents);

            var downloadLocation = new Uri(fileName, UriKind.RelativeOrAbsolute);
            var observer = new MockBackgroundUploadResponseHandlerObserver(expectedResult, fileContents);

            var responseHandler =
                new BackgroundUploadResponseHandler(downloadLocation, observer);

            Action result = responseHandler.OnDoWork();
            result.Invoke();

            observer.CheckOnErrorCalledCorrectly();

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Assert.IsFalse(store.FileExists(fileName), "The created file was not deleted");
            }
        }

        [TestMethod]
        public void TestOnErrorCalledWithException()
        {
            string fileName = "my_file.txt";
            Exception exception = new Exception();

            var downloadLocation = new Uri(fileName, UriKind.RelativeOrAbsolute);
            var observer = new MockBackgroundUploadResponseHandlerObserver(exception);

            var responseHandler = new BackgroundUploadResponseHandler(downloadLocation, observer);
            responseHandler.OnError(exception);

            observer.CheckOnErrorCalledCorrectly();
        }

        private static void CreateFile(string name, string contents)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(name))
                {
                    store.DeleteFile(name);
                }

                using (var stream = store.OpenFile(name, FileMode.CreateNew))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(contents);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
