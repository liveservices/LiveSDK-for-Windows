namespace Microsoft.Live
{
    using System;

    internal static class AuthEndpointsInfo
    {
        /// <summary>
        /// Authorize endpoint path.
        /// </summary>
        public const string AuthorizePath = "/oauth20_authorize.srf";

        /// <summary>
        /// Token endpoint path.
        /// </summary>
        public const string TokenPath = "/oauth20_token.srf";

        /// <summary>
        /// Logout endpoint path.
        /// </summary>
        public const string LogoutPath = "/oauth20_logout.srf";

        /// <summary>
        /// Desktop redirect path.
        /// </summary>
        public const string DesktopRedirectPath = "/oauth20_desktop.srf";

        /// <summary>
        /// Auth host name.
        /// </summary>
        public const string AuthEndpoint = "https://login.live.com";
    }
}
