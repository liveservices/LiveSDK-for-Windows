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
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    [DataContract]
    internal class JsonWebTokenClaims
    {
        [DataMember(Name = "exp")]
        private long expUnixTime
        {
            get;
            set;
        }

        private DateTime? expiration = null;
        public DateTime Expiration
        {
            get
            {
                if (this.expiration == null)
                {
                    this.expiration = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expUnixTime);
                }

                return this.expiration.Value;
            }
        }

        [DataMember(Name = "iss")]
        public string Issuer
        {
            get;
            private set;
        }

        [DataMember(Name = "aud")]
        public string Audience
        {
            get;
            private set;
        }

        [DataMember(Name = "uid")]
        public string UserId
        {
            get;
            private set;
        }

        [DataMember(Name = "ver")]
        public int Version
        {
            get;
            private set;
        }

        [DataMember(Name = "urn:microsoft:appuri")]
        public string ClientIdentifier
        {
            get;
            private set;
        }

        [DataMember(Name = "urn:microsoft:appid")]
        public string AppId
        {
            get;
            private set;
        }
    }
}
