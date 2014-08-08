namespace Microsoft.Live.Phone
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    internal class BackgroundWorker<TResult>
    {
        #region Fields

        private BackgroundWorker worker; 
        private bool exceptionThrown; 
        private readonly IBackgroundWorkerStrategy<TResult> strategy;

        #endregion

        #region Constructors

        public BackgroundWorker(IBackgroundWorkerStrategy<TResult> strategy)
        {
            Debug.Assert(strategy != null);
            this.strategy = strategy;
        }

        #endregion

        #region Methods

        public void Execute() {
            this.worker = new BackgroundWorker
                              {
                                  WorkerReportsProgress = false,
                                  WorkerSupportsCancellation = false
                              };
            this.worker.DoWork += this.WorkerOnDoWork;
            this.worker.RunWorkerCompleted += this.WorkerOnRunWorkerCompleted;
            this.worker.RunWorkerAsync();
        }


        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.worker.RunWorkerCompleted -= this.WorkerOnRunWorkerCompleted;
            if (this.exceptionThrown)
            {
                this.strategy.OnError((Exception) e.Result);
            }
            else
            {
                this.strategy.OnSuccess((TResult) e.Result);
            }
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            this.worker.DoWork -= this.WorkerOnDoWork;
            try
            {
                e.Result = this.strategy.OnDoWork();
            }
            catch (Exception exception)
            {
                this.exceptionThrown = true;
                e.Result = exception;
            }
        }

        #endregion
    }
}
