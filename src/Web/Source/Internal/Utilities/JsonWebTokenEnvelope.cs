namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    [DataContract]
    internal class JsonWebTokenEnvelope
    {
        [DataMember(Name = "typ")]
        public string Type
        {
            get;
            private set;
        }

        [DataMember(Name = "alg")]
        public string Algorithm
        {
            get;
            private set;
        }

        [DataMember(Name = "kid")]
        public int KeyId
        {
            get;
            private set;
        }
    }
}
