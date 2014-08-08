namespace Microsoft.Live
{
    using System;
    using System.Net;

    /// <summary>
    /// An interface that allows test projects to override the outbound web request behavior.
    /// </summary>
    internal interface IWebRequestFactory
    {
        /// <summary>
        /// Create a WebRequest object.
        /// </summary>
        WebRequest CreateWebRequest(Uri url, string method);
    }
}
