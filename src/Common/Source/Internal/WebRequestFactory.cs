namespace Microsoft.Live
{
    using System;
    using System.Net;

    /// <summary>
    /// A factory class that creates the default WebRequest objects.
    /// </summary>
    internal class WebRequestFactory : IWebRequestFactory
    {
        private static IWebRequestFactory requestFactory;

        /// <summary>
        /// Constructor to create a new instance of the class.
        /// </summary>
        private WebRequestFactory()
        {
        }

        public static IWebRequestFactory Current
        {
            get
            {
                return requestFactory ?? (requestFactory = new WebRequestFactory());
            }

            set
            {
                requestFactory = value;
            }
        }

        /// <summary>
        /// Creates a new WebRequest object.
        /// </summary>
        public WebRequest CreateWebRequest(Uri url, string method)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.Accept = "*/*";

            return request;
        }
    }
}
