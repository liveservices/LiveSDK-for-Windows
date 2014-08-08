namespace Microsoft.Live.UnitTests
{
    using System;

    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;
    using Microsoft.Live.Win8.UnitTests.Stubs;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.Storage;

    [TestClass]
    public class TailoredDownloadOperationTest
    {

        // TODO: Test Attach, Execute, and Cancel

        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            var requestUrl = new Uri("http://foo.com");
            IStorageFile outputFile = new StorageFileStub();
            IProgress<LiveOperationProgress> progress = new Progress<LiveOperationProgress>();
            SynchronizationContextWrapper syncContext = SynchronizationContextWrapper.Current;

            var tailoredDownloadOperation =
                new TailoredDownloadOperation(connectClient, requestUrl, outputFile, progress, syncContext);
        }
    }
}
