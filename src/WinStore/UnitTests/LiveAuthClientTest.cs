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
