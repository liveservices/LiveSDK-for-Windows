// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
