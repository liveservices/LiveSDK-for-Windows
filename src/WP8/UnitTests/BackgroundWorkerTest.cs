namespace Microsoft.Live.WP8.UnitTests
{
    using System;

    using Microsoft.Live.Phone;
    using Microsoft.Live.UnitTest.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundWorkerTest
    {
        [TestMethod]
        public void TestCallDoWorkOnBackgroundThreadAndThenCallOnSuccessOnMainThread()
        {
            var strategy = new MockBackgroundWorkerStrategy();
            var operation = new BackgroundWorker<object>(strategy);
            operation.Execute();
            strategy.CheckThatOnDoWorkWasCalled();
        }
    }
}
