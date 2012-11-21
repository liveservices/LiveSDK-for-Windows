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

        internal LiveAuthClient AuthClient { get; set; }

        #endregion
    }
}
