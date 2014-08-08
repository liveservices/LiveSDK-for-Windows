namespace Microsoft.Live.UnitTest
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    internal class MockWebResponse : WebResponse
    {
        private readonly Uri responseUri;
        private readonly long contentLength;
        private readonly MemoryStream responseStream;
        private readonly string contentType;

        public MockWebResponse(Uri responseUri, string responseData)
        {
            this.responseUri = responseUri;
            this.responseStream = new MemoryStream();
            var sw = new StreamWriter(this.responseStream, Encoding.UTF8);
            sw.Write(responseData);
            sw.Flush();
            this.contentLength = this.responseStream.Length;

            this.responseStream.Seek(0, 0);
        }

        public MockWebResponse(Uri responseUri, string responseData, string contentType)
            : this(responseUri, responseData)
        {
            this.contentType = contentType;
        }


        public override Uri ResponseUri
        {
            get
            {
                return this.responseUri;
            }
        }

        public override long ContentLength
        {
            get
            {
                return this.contentLength;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        public override Stream GetResponseStream()
        {
            return this.responseStream;
        }
    }
}
