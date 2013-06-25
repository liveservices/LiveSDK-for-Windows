namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Security.Authentication.OnlineId;

    /// <summary>
    /// Class that implements the platform specific auth flow for Tailored apps.
    /// It is assumed that the calling code ensures only one AuthenticateAsync
    /// call is active at any given time.
    /// </summary>
    internal class TailoredAuthClient : IAuthClient
    {
        private const string Win8ReturnUriScheme = "ms-app";
        private const string Win8AppIdPrefix = Win8ReturnUriScheme + "://";
        private const int UserNotFoundLoginExceptionHResult = -2147023579;
        private const int ConsentNotGrantedExceptionHResult = -2138701812;
        private const int InvalidClientExceptionHResult = -2138701821;
        private const int InvalidAuthTargetExceptionHResult = -2138701823;

        private readonly LiveAuthClient authClient;
        private readonly OnlineIdAuthenticator authenticator;

        /// <summary>
        /// Creates a new TailoredAuthClient class.
        /// </summary>
        /// <param name="authClient">The LiveAuthClient instance.</param>
        public TailoredAuthClient(LiveAuthClient authClient)
        {
            Debug.Assert(authClient != null, "authClient cannot be null.");

            this.authClient = authClient;
            this.authenticator = new OnlineIdAuthenticator();
        }

        /// <summary>
        /// Whether or not the user can be signed out.
        /// </summary>
        public bool CanSignOut
        {
            get
            {
                return this.authenticator.CanSignOut;
            }
        }

        /// <summary>
        /// Authenticate the user.  Ask user for consent if neccessary.
        /// </summary>
        public async Task<LiveLoginResult> AuthenticateAsync(string scopes, bool silent)
        {
            Exception error = null;
            string accessToken = null;
            string authenticationToken = null;

            LiveLoginResult result = null;

            try
            {
                accessToken = await this.GetAccessToken(scopes, silent);
                LiveConnectSession session = new LiveConnectSession(this.authClient);
                session.AccessToken = accessToken;

                if (!string.IsNullOrEmpty(this.authClient.RedirectUrl) &&
                    !this.authClient.RedirectUrl.StartsWith(Win8AppIdPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    authenticationToken = await this.GetAuthenticationToken(this.authClient.RedirectUrl, silent);
                    session.AuthenticationToken = authenticationToken;
                }

                result = new LiveLoginResult(LiveConnectSessionStatus.Connected, session);
            }
            catch (TaskCanceledException)
            {
                result = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
            }
            catch (Exception comExp)
            {
                switch (comExp.HResult)
                {
                    case TailoredAuthClient.UserNotFoundLoginExceptionHResult:
                        result = new LiveLoginResult(LiveConnectSessionStatus.Unknown, null);
                        break;
                    case TailoredAuthClient.ConsentNotGrantedExceptionHResult:
                        result = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, null);
                        break;
                    case TailoredAuthClient.InvalidClientExceptionHResult:
                    case TailoredAuthClient.InvalidAuthTargetExceptionHResult:
                        error = new LiveAuthException(AuthErrorCodes.InvalidRequest, ResourceHelper.GetString("InvalidAuthClient"), comExp);
                        break;
                    default:
                        error = new LiveAuthException(AuthErrorCodes.ServerError, ResourceHelper.GetString("ServerError"), comExp);
                        break;
                }
            }

            if (result == null)
            {
                Debug.Assert(error != null);

                result = new LiveLoginResult(error);
            }

            return result;
        }

        /// <summary>
        /// Load session from persistence store. N/A on Windows 8.
        /// </summary>
        public LiveConnectSession LoadSession(LiveAuthClient authClient)
        {
            // We don't store any session data on Win8.
            return null;
        }

        /// <summary>
        /// Save session to persistence store. N/A on Windows 8.
        /// </summary>
        public void SaveSession(LiveConnectSession session)
        {
            // No-op.
        }

        /// <summary>
        /// Log out the user.
        /// </summary>
        public async void CloseSession()
        {
            await this.authenticator.SignOutUserAsync();
        }

        private async Task<string> GetAccessToken(string scopes, bool silent)
        {
            string ticket = string.Empty;
            Debug.Assert(!string.IsNullOrEmpty(scopes), "scopes is null or empty.");

            CredentialPromptType promptType = silent ? CredentialPromptType.DoNotPrompt : CredentialPromptType.PromptIfNeeded;
            var ticketRequests = new List<OnlineIdServiceTicketRequest>();
            ticketRequests.Add(new OnlineIdServiceTicketRequest(scopes, "DELEGATION"));

            UserIdentity identity = await this.authenticator.AuthenticateUserAsync(ticketRequests, promptType);
            if (identity.Tickets != null && identity.Tickets.Count > 0)
            {
                ticket = identity.Tickets[0].Value;
            }

            return ticket;
        }

        private async Task<string> GetAuthenticationToken(string redirectDomain, bool silent)
        {
            string ticket = string.Empty;

            var redirectUri = new Uri(redirectDomain, UriKind.Absolute);

            var ticketRequests = new List<OnlineIdServiceTicketRequest>();
            ticketRequests.Add(new OnlineIdServiceTicketRequest(redirectUri.DnsSafeHost, "JWT"));

            var promptType = silent ? CredentialPromptType.DoNotPrompt : CredentialPromptType.PromptIfNeeded;
            UserIdentity identity = await this.authenticator.AuthenticateUserAsync(ticketRequests, promptType);
            if (identity.Tickets != null && identity.Tickets.Count > 0)
            {
                ticket = identity.Tickets[0].Value;
            }

            return ticket;
        }
    }
}