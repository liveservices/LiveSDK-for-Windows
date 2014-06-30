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

    using Windows.System.Threading;

    internal class MockWebRequest : WebRequest
    {
        private readonly Uri url;
        private string method;
        private string contentType;
        private MockNetworkStream requestStream;
        private MockWebResponse response;
        private readonly WebHeaderCollection headers;

        public MockWebRequest(Uri url)
        {
            this.url = url;
            this.headers = new WebHeaderCollection();
        }

        public override Uri RequestUri
        {
            get
            {
                return this.url;
            }
        }

        public override string Method
        {
            get
            {
                return this.method;
            }
            set
            {
                this.method = value;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.contentType;
            }
            set
            {
                this.contentType = value;
            }
        }

        public MockNetworkStream RequestStream
        {
            get
            {
                return this.requestStream;
            }
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                return this.headers;
            }

            set
            {
            }
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            var ar = new MockAsyncResult(callback, state);
            ThreadPool.RunAsync(
                delegate
                    {
                        this.requestStream = new MockNetworkStream();
                        ar.Complete(false);
                    });

            return ar;
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            var ar = new MockAsyncResult(callback, state);
            ThreadPool.RunAsync(
                delegate
                    {
                        Exception error = null;
                        string result = null;
                        try
                        {
                            result = FakeService.ProcessRequest(this);
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }

                        if (error != null)
                        {
                            ar.HandleFailure(error, false);
                        }
                        else
                        {
                            this.response = new MockWebResponse(this.url, result);
                            ar.Complete(false);
                        }
                    });

            return ar;
        }

        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.requestStream;
        }

        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return this.response;
        }
    }
}
