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
