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

namespace Microsoft.Live.Controls
{
    using System;

    /// <summary>
    /// Event argument for SessionChanged event.
    /// </summary>
    public class LiveConnectSessionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new LiveConnectSessionChangedEventArgs object.
        /// </summary>
        /// <param name="newSession">A new LiveConnectSession instance.</param>
        public LiveConnectSessionChangedEventArgs(LiveConnectSessionStatus newStatus, LiveConnectSession newSession)
        {
            this.Session = newSession;
            this.Status = newStatus;
        }

        /// <summary>
        /// Construct a new LiveConnectSessionChangedEventArgs object.
        /// </summary>
        /// <param name="error">The Exception object generated during the login process.</param>
        public LiveConnectSessionChangedEventArgs(Exception error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the error object.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the LiveConnectSession object.
        /// </summary>
        public LiveConnectSession Session { get; private set; }

        /// <summary>
        /// Gets the login status.
        /// </summary>
        public LiveConnectSessionStatus Status { get; internal set; }
    }
}
