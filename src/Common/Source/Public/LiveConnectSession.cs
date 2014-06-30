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
