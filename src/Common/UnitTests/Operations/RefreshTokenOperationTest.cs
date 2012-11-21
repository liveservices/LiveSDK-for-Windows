namespace Microsoft.Live.UnitTests
{
    using System;
    using System.Collections.Generic;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;

    [TestClass]
    public class RefreshTokenOperationTest
    {
        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();

#if NETFX_CORE
            LiveAuthClient authClient = new LiveAuthClient();
#elif WINDOWS_PHONE
            LiveAuthClient authClient = new LiveAuthClient("clientId");
#else
#error This platform needs to be handled.
#endif

            string clientId = "clientId";
            string refreshToken = "refreshToken";
            IEnumerable<string> scopes = new string[]{ "wl.basic" };
            SynchronizationContextWrapper syncContext = SynchronizationContextWrapper.Current;

            var refreshOperation =
                new RefreshTokenOperation(authClient, clientId, refreshToken, scopes, syncContext);
        }
    }
}
