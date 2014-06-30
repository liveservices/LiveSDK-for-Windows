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
