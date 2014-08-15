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

namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// This class contains the result of an auth operation.
    /// </summary>
    public class LiveLoginResult
    {
        #region Constructors

        /// <summary>
        /// Creates a new LiveLoginResult object.
        /// </summary>
        /// <param name="status">Connect status.</param>
        /// <param name="session">The session object if the status is Connected.</param>
        internal LiveLoginResult(LiveConnectSessionStatus status, LiveConnectSession session)
        {
            this.Status = status;
            this.Session = session;
        }


        /// <summary>
        /// Creates a new LiveLoginResult object when login fails.
        /// </summary>
        /// <param name="error">The exception that occured.</param>
        internal LiveLoginResult(Exception error)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the session object if the user is successfully connected.
        /// </summary>
        public LiveConnectSession Session { get; private set; }

        /// <summary>
        /// Gets the connect status.
        /// </summary>
        public LiveConnectSessionStatus Status { get; private set; }

        /// <summary>
        /// Gets the request state string value from the current auth request set via the 
        /// LiveAuthClient.GetLoginUrl(...) or WL.login(...) from the JavaScript library.
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Gets the login error code.  This corresponds to the OAuth error parameter.
        /// </summary>
        internal Exception Error { get; private set; }

        #endregion
    }
}
