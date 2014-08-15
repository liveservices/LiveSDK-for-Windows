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

namespace Microsoft.Live.Win8.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Windows.Storage.Streams;

    [TestClass]
    public class LiveConnectClientTest
    {

        #region Constructor tests

        [TestMethod]
        public void TestConstructorNullLiveConnectSession()
        {
            try
            {
                new LiveConnectClient(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        #endregion

        #region Get tests

        [TestMethod]
        public void TestGetNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.GetAsync(null);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void TestGetEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.GetAsync(string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestGetWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.GetAsync("\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Delete tests

        [TestMethod]
        public void TestDeleteNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.DeleteAsync(null);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestDeleteEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.DeleteAsync(string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestDeleteWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.DeleteAsync("\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Put tests

        [TestMethod]
        public void TestPutNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync(null, new Dictionary<string, object>());
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestPutEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync(string.Empty, new Dictionary<string, object>());
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPutWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync("\t\n ", new Dictionary<string, object>());
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPutNullDictionaryBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                IDictionary<string, object> body = null;
                connectClient.PutAsync("fileId.123", body);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void TestPutNullStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                string body = null;
                connectClient.PutAsync("fileId.123", body);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestPutEmptyStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync("fileId.123", string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPutWhiteSpaceStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync("fileId.123", "\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Post tests

        [TestMethod]
        public void TestPostNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PostAsync(null, new Dictionary<string, object>());
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestPostEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PostAsync(string.Empty, new Dictionary<string, object>());
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPostWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PostAsync("\t\n ", new Dictionary<string, object>());
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPostNullDictionaryBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                IDictionary<string, object> body = null;
                connectClient.PostAsync("fileId.123", body);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void TestPostNullStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                string body = null;
                connectClient.PostAsync("fileId.123", body);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestPostEmptyStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PostAsync("fileId.123", string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestPostWhiteSpaceStringBody()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.PutAsync("fileId.123", "\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Copy tests

        [TestMethod]
        public void TestCopyNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync(null, "fileId.123");
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestCopyEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync(string.Empty, "fileId.123");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestCopyWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync("\t\n ", "fileId.123");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestCopyNullDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync("fileId.123", null);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestCopyEmptyStringDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync("fileId.123", string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestCopyWhiteSpaceStringDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.CopyAsync("fileId.123", "\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Move tests

        [TestMethod]
        public void TestMoveNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync(null, "fileId.123");
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestMoveEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync(string.Empty, "fileId.123");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestMoveWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync("\t\n ", "fileId.123");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestMoveNullDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync("fileId.123", null);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestMoveEmptyStringDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync("fileId.123", string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestMoveWhiteSpaceStringDestination()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.MoveAsync("fileId.123", "\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Download tests

        [TestMethod]
        public void TestDownloadNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.BackgroundDownloadAsync(null);
                Assert.Fail("Expected ArguementNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void TestDownloadEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.BackgroundDownloadAsync(string.Empty);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestDownloadWhiteSpaceStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.BackgroundDownloadAsync("\t\n ");
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Upload tests

        [TestMethod]
        public void TestUploadNullPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.BackgroundUploadAsync(
                    null, 
                    "fileName.txt", 
                    new InMemoryRandomAccessStream(), 
                    OverwriteOption.Overwrite);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestUploadEmptyStringPath()
        {
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            try
            {
                connectClient.BackgroundUploadAsync(
                    string.Empty, 
                    "fileName.txt", 
                    new InMemoryRandomAccessStream(), 
                    OverwriteOption.Overwrite);
                Assert.Fail("Expected ArguementException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion
    }
}
