namespace Microsoft.Live
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface that defines the contract betwewn the LiveAuthClient class and the platform
    /// specific implementation of the authentication flow.
    /// </summary>
    internal interface IAuthClient
    {
        /// <summary>
        /// Gets a boolean indicating whether or not the user can be signed out.
        /// </summary>
        bool CanSignOut { get; }

#if NETFX_CORE
        /// <summary>
        /// Performs authentication flow using platform specific technology.
        /// </summary>
        Task<LiveLoginResult> AuthenticateAsync(string scopes, bool silent);
#else
        /// <summary>
        /// Performs authentication flow using platform specific technology.
        /// </summary>
        void AuthenticateAsync(string clientId, string scopes, bool silent, Action<string, Exception> callback);
#endif

        /// <summary>
        /// Saves the session data to app local storage.
        /// </summary>
        LiveConnectSession LoadSession(LiveAuthClient authClient);

        /// <summary>
        /// Loads the session data from app local storage.
        /// </summary>
        void SaveSession(LiveConnectSession session);

        /// <summary>
        /// Clears the session data in app local storage.
        /// </summary>
        void CloseSession();
    }
}
