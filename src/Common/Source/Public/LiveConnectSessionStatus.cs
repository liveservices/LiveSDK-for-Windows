namespace Microsoft.Live
{
    using System;

    public enum LiveConnectSessionStatus
    {
        Unknown,
        Connected,
        NotConnected,
#if WEB
        Expired,
#endif
    }
}
