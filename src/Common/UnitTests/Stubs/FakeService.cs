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
    using System.Collections.Generic;
    using System.Globalization;

    using Microsoft.Live;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
    using System.Net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    internal class FakeService
    {
        private const string ContentTypeFormEncoded = @"application/x-www-form-urlencoded";
        private const string ContentTypeJson = @"application/json;charset=UTF-8";
        private static readonly string ApiEndpoint = "/v5.0/";
        private static readonly string OAuthTokenEndpoint = "/oauth20_token.srf";

        private enum ResourceType
        {
            Me,
            Contacts,
            Contact,
            File,
            Folder,
            Files,
        }


        public static string ProcessRequest(MockWebRequest request)
        {
            string responseData = null;

            if (request.RequestUri.AbsolutePath.StartsWith(FakeService.OAuthTokenEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                Assert.IsTrue(request.ContentType.Equals(ContentTypeFormEncoded, StringComparison.OrdinalIgnoreCase));

                string postBody = request.RequestStream.Content;
                Assert.IsTrue(!string.IsNullOrEmpty(postBody));

                IDictionary<string, string> parameters = Helpers.ParseQueryString(postBody);
                string oldRefreshToken = parameters[AuthConstants.RefreshToken];
                Assert.IsTrue(oldRefreshToken.Equals(TestAuthClient.FakeOldRefreshToken, StringComparison.OrdinalIgnoreCase));

                return string.Format(
                        CultureInfo.InvariantCulture,
                        ResponseJson.RefreshTokenResponse,
                        TestAuthClient.FakeAccessToken,
                        TestAuthClient.FakeRefreshToken,
                        HttpUtility.UrlDecode(LiveAuthClient.BuildScopeString(TestAuthClient.Scopes)));
            }
            else if (request.RequestUri.AbsolutePath.StartsWith(FakeService.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                string authHeader = request.Headers["Authorization"];
                Assert.IsTrue(!string.IsNullOrEmpty(authHeader));
                Assert.IsTrue(authHeader.StartsWith("bearer "));

                ResourceType resourceType = GetResourceType(request.RequestUri);

                if (resourceType == ResourceType.Me)
                {
                    if (request.Method != "GET")
                    {
                        responseData = ResponseJson.ErrorInvalidMethod;
                    }
                    else
                    {
                        Assert.IsTrue(request.RequestStream == null);

                        responseData = ResponseJson.Me;
                    }
                }
                else if (resourceType == ResourceType.Contact)
                {
                    if (request.Method == "GET")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = ResponseJson.Contact;
                    }
                    else if (request.Method == "PUT")
                    {
                        Assert.IsNotNull(request.RequestStream);
                        Assert.IsNotNull(request.RequestStream.Content);
                        Assert.IsTrue(request.ContentType.Equals(ContentTypeJson));

                        Assert.IsTrue(request.RequestStream.Content.Equals(ResponseJson.Contact, StringComparison.OrdinalIgnoreCase), "Request body not written correctly.");

                        responseData = ResponseJson.Contact;
                    }
                    else if (request.Method == "DELETE")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = string.Empty;
                    }
                    else
                    {
                        responseData = ResponseJson.ErrorInvalidMethod;
                    }
                }
                else if (resourceType == ResourceType.Contacts)
                {
                    if (request.Method == "GET")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = ResponseJson.Contacts;
                    }
                    else if (request.Method == "POST")
                    {
                        Assert.IsTrue(request.ContentType.Equals(ContentTypeJson));
                        Assert.IsNotNull(request.RequestStream);
                        Assert.IsNotNull(request.RequestStream.Content);
                        Assert.IsTrue(request.RequestStream.Content.Equals(ResponseJson.Contact, StringComparison.OrdinalIgnoreCase), "Request body not written correctly.");

                        responseData = ResponseJson.Contact;
                    }
                    else
                    {
                        responseData = ResponseJson.ErrorInvalidMethod;
                    }
                }
                else if (resourceType == ResourceType.File)
                {
                    if (request.Method == "GET")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = ResponseJson.File;
                    }
                    else if (request.Method == "PUT")
                    {
                        Assert.IsTrue(request.ContentType.Equals(ContentTypeJson));
                        Assert.IsNotNull(request.RequestStream);
                        Assert.IsNotNull(request.RequestStream.Content);
                        Assert.IsTrue(request.RequestStream.Content.Equals(ResponseJson.File, StringComparison.OrdinalIgnoreCase), "Request body not written correctly.");

                        responseData = ResponseJson.File;
                    }
                    else if (request.Method == "DELETE")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = string.Empty;
                    }
                    else if (request.Method == "MOVE" || request.Method == "COPY")
                    {
                        Assert.IsTrue(request.ContentType.Equals(ContentTypeJson));
                        Assert.IsNotNull(request.RequestStream);
                        Assert.IsNotNull(request.RequestStream.Content);

                        responseData = ResponseJson.File;
                    }
                    else
                    {
                        responseData = ResponseJson.ErrorInvalidMethod;
                    }
                }
                else if (resourceType == ResourceType.Files)
                {
                    if (request.Method == "GET")
                    {
                        Assert.IsTrue(request.RequestStream == null);
                        responseData = ResponseJson.Files;
                    }
                    else if (request.Method == "POST")
                    {
                        Assert.IsTrue(request.ContentType.Equals(ContentTypeJson));
                        Assert.IsNotNull(request.RequestStream);
                        Assert.IsNotNull(request.RequestStream.Content);
                        Assert.IsTrue(request.RequestStream.Content.Equals(ResponseJson.File, StringComparison.OrdinalIgnoreCase), "Request body not written correctly.");

                        responseData = ResponseJson.Files;
                    }
                    else
                    {
                        responseData = ResponseJson.ErrorInvalidMethod;
                    }
                }
            }

            return responseData;
        }

        private static ResourceType GetResourceType(Uri requestUri)
        {
            ResourceType resourceType = ResourceType.Me;
            string path = requestUri.AbsolutePath.TrimEnd('/');
            string[] fragments = path.Split('/');
            string resourceName = fragments[fragments.Length - 1];

            if (resourceName.Equals("me", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.Me;
            }
            else if (resourceName.Equals("contacts", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.Contacts;    
            }
            else if (resourceName.StartsWith("contact.", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.Contact;
            }
            else if (resourceName.Equals("files", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.Files;
            }
            else if (resourceName.StartsWith("file.", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.File;
            }
            else if (resourceName.StartsWith("folder.", StringComparison.OrdinalIgnoreCase))
            {
                resourceType = ResourceType.Folder;
            }
            else
            {
                Assert.Fail("unsupported resource type.");
            }

            return resourceType;
        }

        private static string GenerateTokenReturnResponse(string scopes)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                ResponseJson.RefreshTokenResponse,
                TestAuthClient.FakeAccessToken,
                TestAuthClient.FakeRefreshToken,
                scopes);
        }
    }
}
