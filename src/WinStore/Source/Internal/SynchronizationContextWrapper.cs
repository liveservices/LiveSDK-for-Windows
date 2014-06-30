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
