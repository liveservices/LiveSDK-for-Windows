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
