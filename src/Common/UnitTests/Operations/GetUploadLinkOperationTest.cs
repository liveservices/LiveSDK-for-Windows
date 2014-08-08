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
    public class GetUploadLinkOperationTest
    {

        // TODO: Test Execute, Cancel, and ExecuteAsync

        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();
            LiveConnectClient connectClient = new LiveConnectClient(new LiveConnectSession());
            Uri requestUri = new Uri("http://foo.com");
            string fileName = string.Empty;
            OverwriteOption overwriteOption = OverwriteOption.Overwrite;
            SynchronizationContextWrapper syncContextWrapper = SynchronizationContextWrapper.Current;

            var apiOperation =
                new GetUploadLinkOperation(connectClient, requestUri, fileName, overwriteOption, syncContextWrapper);
        }
    }
}
