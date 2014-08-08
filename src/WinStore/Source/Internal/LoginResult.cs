namespace Microsoft.Live
{
    using System;

    internal class LoginResult
    {
        public LoginResult()
        {
        }

        public LiveConnectSessionStatus Status { get; internal set; }

        public LiveConnectSession Session { get; internal set; }

        public string ErrorCode { get; internal set; }

        public string ErrorDescription { get; internal set; }
    }
}
