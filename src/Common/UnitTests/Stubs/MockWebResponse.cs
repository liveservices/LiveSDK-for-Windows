// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

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
