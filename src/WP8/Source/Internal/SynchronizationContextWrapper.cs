namespace Microsoft.Live
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    internal class SynchronizationContextWrapper
    {
        private static SynchronizationContextWrapper syncContxtWrapper;
        private readonly SynchronizationContext syncContext;

        public SynchronizationContextWrapper(SynchronizationContext syncContext)
        {
            this.syncContext = syncContext;
        }

        static public SynchronizationContextWrapper Current
        {
            get
            {
                SynchronizationContextWrapper.syncContxtWrapper = new SynchronizationContextWrapper(SynchronizationContext.Current);

                return SynchronizationContextWrapper.syncContxtWrapper;
            }
        }

        public void Post(Action callback)
        {
            Debug.Assert(callback != null);

            if (this.syncContext != null)
            {
                this.syncContext.Post(
                    delegate(object state)
                    {
                        callback();
                    }, null);
            }
            else
            {
                callback();
            }
        }
    }
}
