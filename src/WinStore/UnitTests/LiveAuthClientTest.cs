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
