// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
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
