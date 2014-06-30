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
