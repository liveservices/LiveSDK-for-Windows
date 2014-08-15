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
    using System.Collections.Generic;

    public class LiveConnectSession
    {
        private readonly static TimeSpan ExpirationTimeBufferInSec = new TimeSpan(0, 0, 60);

        #region Constructors

        internal LiveConnectSession(LiveAuthClient authClient)
        {
            this.AuthClient = authClient;
        }

        internal LiveConnectSession()
        {
        }

        #endregion

        #region Properties

        public string AccessToken { get; internal set; }

        public string AuthenticationToken { get; internal set; }

#if !WINDOWS_STORE
        public DateTimeOffset Expires { get; internal set; }
        
        public string RefreshToken { get; internal set; }

        public IEnumerable<string> Scopes { get; internal set; }

        internal bool IsValid
        {
            get
            {
                return (!string.IsNullOrEmpty(this.AccessToken) &&
                    this.Expires > DateTimeOffset.UtcNow.Add(ExpirationTimeBufferInSec));
            }
        }
#endif
        internal LiveAuthClient AuthClient { get; set; }

        #endregion
    }
}
