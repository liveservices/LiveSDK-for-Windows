namespace Microsoft.Live.UnitTest
{
    using System;
    using System.Net;
    using Microsoft.Live;

    public class TestWebRequestFactory : IWebRequestFactory
    {
        public WebRequest CreateWebRequest(Uri url, string method)
        {
            var request = new MockWebRequest(url);
            request.Method = method;

            return request;
        }
    }
}
