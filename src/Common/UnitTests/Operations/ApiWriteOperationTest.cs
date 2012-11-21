namespace Microsoft.Live.UnitTests
{
    using System;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WEB
    using VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_PHONE
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif
    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;

    [TestClass]
    public class ApiWriteOperationTest
    {

        // TODO: Test public methods: Execute and Cancel

        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();
            LiveConnectClient connectClient = new LiveConnectClient(new LiveConnectSession());
            Uri requestUri = new Uri("http://foo.com");
            ApiMethod apiMethod = ApiMethod.Copy;
            string body = string.Empty;
            SynchronizationContextWrapper syncContextWrapper = SynchronizationContextWrapper.Current;

            var apiOperation =
                new ApiWriteOperation(connectClient, requestUri, apiMethod, body, syncContextWrapper);
        }
    }
}
