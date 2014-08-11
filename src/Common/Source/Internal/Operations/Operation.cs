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

namespace Microsoft.Live.Operations
{
    using System;
    using System.Diagnostics;

    internal abstract class Operation
    {
        #region Constructors

        protected Operation(SynchronizationContextWrapper syncContext)
        {
            this.Status = OperationStatus.NotStarted;
            this.Dispatcher = syncContext ?? SynchronizationContextWrapper.Current;
        }

        #endregion

        #region Properties

        public SynchronizationContextWrapper Dispatcher { get; private set; }

        public OperationStatus Status { get; internal set; }

        public bool IsCancelled
        {
            get
            {
                return this.Status == OperationStatus.Cancelled;
            }
        }

        #endregion

        #region Public methods

        public void Execute()
        {
            Debug.Assert(this.Status != OperationStatus.Started, "Cannot re-execute a started operation.");
            Debug.Assert(this.Status != OperationStatus.Completed, "Cannot re-execute a completed operation.");

            if (this.IsCancelled)
            {
                // This operation already has been cancelled and its OnCancel method should of already been called.
                return;
            }

            this.Status = OperationStatus.Started;

            this.InternalExecute();
        }

        public virtual void Cancel()
        {
            if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
            {
                // no-op
                return;
            }

            this.Status = OperationStatus.Cancelled;
        }

        #endregion

        #region Protected methods

        protected abstract void OnExecute();

        protected abstract void OnCancel();

        protected void InternalExecute()
        {
            if (this.IsCancelled)
            {
                this.OnCancel();
            }
            else
            {
                this.OnExecute();
            }
        }

        #endregion
    }
}
