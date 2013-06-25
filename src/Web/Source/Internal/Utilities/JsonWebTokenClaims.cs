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
