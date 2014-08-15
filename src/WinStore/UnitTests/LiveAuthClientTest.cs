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

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    [TestClass]
    public class LiveAuthClientTest
    {
        #region Constructor tests

        [TestMethod]
        public void TestConstructorInvalidRedirectUrl()
        {
            try
            {
                new LiveAuthClient("some invalid redirect url");
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }


        [TestMethod]
        public void TestConstructorNullRedirectUrl()
        {
            // null redirect url is allowed.
            new LiveAuthClient(null);
        }

        [TestMethod]
        public void TestConstructorEmptyStringRedirectUrl()
        {
            // empty string redirect url is allowed.
            new LiveAuthClient(string.Empty);
        }

        #endregion

        #region Logout tests

        [TestMethod]
        public void TestCanLogout()
        {
            var authClient = new LiveAuthClient(string.Empty);
            Assert.IsFalse(authClient.CanLogout);
        }

        #endregion

        #region Initialize tests

        [TestMethod]
        public void TestInitializeAsyncNullScopes()
        {
            var authClient = new LiveAuthClient();
            try
            {
                authClient.InitializeAsync(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        #endregion

        #region Login tests

        [TestMethod]
        public void TestLoginAsyncNullScopes()
        {
            var authClient = new LiveAuthClient();
            try
            {
                authClient.LoginAsync(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        #endregion

        // TODO: Mock out LiveAuthClient's dependencies and test it more

    }
}
