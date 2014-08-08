namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// This class contains the result of an auth operation.
    /// </summary>
    public class LiveLoginResult
    {
        #region Constructors

        /// <summary>
        /// Creates a new LiveLoginResult object.
        /// </summary>
        /// <param name="status">Connect status.</param>
        /// <param name="session">The session object if the status is Connected.</param>
        internal LiveLoginResult(LiveConnectSessionStatus status, LiveConnectSession session)
        {
            this.Status = status;
            this.Session = session;
        }


        /// <summary>
        /// Creates a new LiveLoginResult object when login fails.
        /// </summary>
        /// <param name="error">The exception that occured.</param>
        internal LiveLoginResult(Exception error)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the session object if the user is successfully connected.
        /// </summary>
        public LiveConnectSession Session { get; private set; }

        /// <summary>
        /// Gets the connect status.
        /// </summary>
        public LiveConnectSessionStatus Status { get; private set; }

        /// <summary>
        /// Gets the request state string value from the current auth request set via the 
        /// LiveAuthClient.GetLoginUrl(...) or WL.login(...) from the JavaScript library.
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Gets the login error code.  This corresponds to the OAuth error parameter.
        /// </summary>
        internal Exception Error { get; private set; }

        #endregion
    }
}
