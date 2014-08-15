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
