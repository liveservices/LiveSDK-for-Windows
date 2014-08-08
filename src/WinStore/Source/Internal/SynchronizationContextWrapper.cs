namespace Microsoft.Live
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Windows.UI.Core;
    using Windows.UI.Xaml;

    internal class SynchronizationContextWrapper
    {
        private readonly CoreDispatcher syncContext;

        public SynchronizationContextWrapper(CoreDispatcher syncContext)
        {
            this.syncContext = syncContext;
        }

        static public SynchronizationContextWrapper Current
        {
            get
            {
                try
                {
                    if (Window.Current != null && Window.Current.Dispatcher != null)
                    {
                        return new SynchronizationContextWrapper(Window.Current.Dispatcher);
                    }
                }
                catch (COMException)
                {
                }

                return new SynchronizationContextWrapper(null);
            }
        }

        public async void Post(Action callback)
        {
            Debug.Assert(callback != null);

            if (this.syncContext != null)
            {
                await this.syncContext.RunAsync(
                    CoreDispatcherPriority.Normal,
                    delegate()
                    {
                        callback();
                    });
            }
            else
            {
                callback();
            }
        }
    }
}
