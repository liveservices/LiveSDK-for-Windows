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
