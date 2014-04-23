namespace Microsoft.Live.UnitTests
{
    using System;
    using System.Threading;

    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;
    using Microsoft.Live.Win8.UnitTests.Stubs;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.Storage;

    [TestClass]
    public class TailoredUploadOperationTest
    {

        // TODO: Test Attach, Execute, and Cancel

        [TestMethod]
        public void TestExecuteMethod()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();
            LiveConnectClient connectClient = new LiveConnectClient(new LiveConnectSession());
            Uri requestUri = new Uri("http://foo.com");
            string fileName = "fileName.txt";
            IStorageFile inputFile = new StorageFileStub();
            OverwriteOption option = OverwriteOption.Overwrite;
            IProgress<LiveOperationProgress> progress = new Progress<LiveOperationProgress>();
            SynchronizationContextWrapper syncContext = SynchronizationContextWrapper.Current;

            var tailoredUploadOperation = new TailoredUploadOperation(
                connectClient,
                requestUri,
                fileName,
                inputFile,
                option,
                progress,
                syncContext);
        }
    }
}
