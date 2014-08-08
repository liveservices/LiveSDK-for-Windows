namespace Microsoft.Live.UnitTest.Stubs
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    class MockBackgroundWorkerStrategy : IBackgroundWorkerStrategy<object>
    {
        private bool doWorkCalled;
        private readonly object doWorkCalledLock = new object();

        private readonly object result = new object();

        public object OnDoWork()
        {
            lock (this.doWorkCalledLock)
            {
                this.doWorkCalled = true;
                Monitor.Pulse(this.doWorkCalledLock);
            }

            return result;
        }

        public void OnSuccess(object result)
        {
            // Asserts here will crash the app
        }

        public void OnError(Exception exception)
        {
            // Asserts here will crash the app
        }

        internal void CheckThatOnDoWorkWasCalled()
        {
            lock (this.doWorkCalledLock)
            {
                if (!this.doWorkCalled)
                {
                    Monitor.Wait(this.doWorkCalledLock, 1000);
                }

                Assert.IsTrue(this.doWorkCalled, "The OnDoWork callback was never called.");
            }
        }
    }
}
