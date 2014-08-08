namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// Defines the error codes used by the library.
    /// These error codes match the ones used on the server.
    /// </summary>
    internal static class AuthErrorCodes
    {
        public const string AccessDenied = "access_denied";
        public const string ClientError = "client_error";
        public const string InvalidGrant = "invalid_grant";
        public const string InvalidRequest = "invalid_request";
        public const string ServerError = "server_error";
        public const string UnknownUser = "unknown_user";
        public const string SessionExpired = "session_expired";
    }
}