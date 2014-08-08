namespace Microsoft.Live.WP8.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LiveAuthClientTest
    {
        #region Constructor tests

        [TestMethod]
        public void TestConstructorNullClientId()
        {
            try
            {
                new LiveAuthClient(null);
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestConstructorEmptyStringClientId()
        {
            try
            {
                new LiveAuthClient(string.Empty);
                Assert.Fail("Expected ArgumentException to be thrown.");
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Initialize tests

        // TODO: Mock out LiveAuthClient's InitializeAsync dependencies and test InitailizeAsync
        //[TestMethod]
        //public void TestInitializeAsync()
        //{
        //}

        #endregion

        #region Login tests

        [TestMethod]
        public async void TestLoginAsyncNullScopes()
        {
            var authClient = new LiveAuthClient("SomeClientId");
            try
            {
                LiveLoginResult result = await authClient.LoginAsync(null);
                Assert.Fail("Expected ArgumentNullException to be thrown.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        #endregion

        #region Logout tests

        // TODO: Mock out LiveAuthClient's Logout dependencies so it can be tested.
        //[TestMethod]
        //public void TestLogout()
        //{
        //}

        #endregion
    }
}
