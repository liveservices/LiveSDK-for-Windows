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
