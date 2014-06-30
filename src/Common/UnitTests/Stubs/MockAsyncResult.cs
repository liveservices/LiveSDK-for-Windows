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

namespace Microsoft.Live.UnitTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    internal class MockAsyncResult : IAsyncResult
    {
        private readonly object syncObj = new object();
        private readonly AsyncCallback userCallback;
        private readonly object userState;
        private ManualResetEvent asyncWait;
        private Exception failure;
        private bool completedSynchronously;
        private bool completed;

        /// <summary>
        /// Intializes a new AsyncResult object.
        /// </summary>
        /// <param name="callback">user callback to invoke when complete</param>
        /// <param name="state">user state</param>
        internal MockAsyncResult(AsyncCallback callback, object state)
        {
            this.userCallback = callback;
            this.userState = state;
        }

        #region IAsyncResult implmentation - AsyncState, AsyncWaitHandle, CompletedSynchronously, IsCompleted

        /// <summary>user state object parameter</summary>
        public object AsyncState
        {
            get { return this.userState; }
        }

        /// <summary>wait handle for when waiting is required</summary>
        /// <remarks>if displayed by debugger, it undesirable to create the WaitHandle</remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this.asyncWait == null)
                {
                    // delay create the wait handle since the user may never use it
                    lock (this.syncObj)
                    {
                        if (this.asyncWait == null)
                        {
                            this.asyncWait = new ManualResetEvent(this.IsCompleted);
                        }
                    }
                }

                return this.asyncWait;
            }
        }

        /// <summary>did the result complete synchronously?</summary>
        public bool CompletedSynchronously
        {
            get { return this.completedSynchronously; }
        }

        /// <summary>is the result complete?</summary>
        public bool IsCompleted
        {
            get { return this.completed; }
        }

        #endregion

        /// <summary>first exception that happened</summary>
        internal Exception Failure
        {
            get { return this.failure; }
        }

        internal void Complete(bool completedSynchronously)
        {
            lock (this.syncObj)
            {
                this.SetCompleted(completedSynchronously);
                this.CompletedRequest();
            }

            this.SignalCompleted();
        }

        /// <summary>Cache the exception that happened on the background thread for the caller of EndSaveChanges.</summary>
        /// <param name="e">exception object from background thread</param>
        /// <returns>true if the exception (like StackOverflow or ThreadAbort) should be rethrown</returns>
        internal void HandleFailure(Exception e, bool completedSynchronously)
        {
            lock (this.syncObj)
            {
                this.failure = e;
                this.SetCompleted(completedSynchronously);
            }

            this.SignalCompleted();
        }

        /// <summary>Set the AsyncWait and invoke the user callback.</summary>
        private void SignalCompleted()
        {
            if (this.asyncWait != null)
            {
                this.asyncWait.Set();
            }

            // invoke the callback because user may throw an exception and stop any further processing
            // TODO(skrueger): See if we still need ThreadAbourtException.
            // if ((this.userCallback != null) && !(this.failure is ThreadAbortException))
            if (this.userCallback != null)
            {   
                // any exception thrown by user should be "unhandled"
                // it's possible callback will be invoked while another creates and sets the asyncWait
                this.userCallback(this);
            }
        }

        /// <summary>Set the async result as completed.</summary>
        internal void SetCompleted(bool completedSynchronously)
        {
            this.completed = true;
            this.completedSynchronously = completedSynchronously;
        }

        /// <summary>invoked for derived classes to cleanup before callback is invoked</summary>
        protected virtual void CompletedRequest()
        {
        }
    }
}