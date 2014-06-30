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
