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
