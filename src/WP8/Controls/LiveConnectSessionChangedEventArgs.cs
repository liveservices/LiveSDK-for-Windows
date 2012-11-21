namespace Microsoft.Live.Controls
{
    using System;

    /// <summary>
    /// Event argument for SessionChanged event.
    /// </summary>
    public class LiveConnectSessionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a new LiveConnectSessionChangedEventArgs object.
        /// </summary>
        /// <param name="newSession">A new LiveConnectSession instance.</param>
        public LiveConnectSessionChangedEventArgs(LiveConnectSessionStatus newStatus, LiveConnectSession newSession)
        {
            this.Session = newSession;
            this.Status = newStatus;
        }

        /// <summary>
        /// Construct a new LiveConnectSessionChangedEventArgs object.
        /// </summary>
        /// <param name="error">The Exception object generated during the login process.</param>
        public LiveConnectSessionChangedEventArgs(Exception error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the error object.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the LiveConnectSession object.
        /// </summary>
        public LiveConnectSession Session { get; private set; }

        /// <summary>
        /// Gets the login status.
        /// </summary>
        public LiveConnectSessionStatus Status { get; internal set; }
    }
}
