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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Live.Web.UnitTests
{
    [TestClass]
    public class LiveAuthClientUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveAuthClient_Constructor_NullClientId()
        {
            new LiveAuthClient(null, "http://www.foo.com", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_Constructor_WhiteSpaceClientId()
        {
            new LiveAuthClient(" ", "http://www.foo.com", null);
        }
                       
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveAuthClient_Constructor_NullClientSecret()
        {
            new LiveAuthClient("000000004802B729", (string)null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_Constructor_WhiteSpaceClientSecret()
        {
            new LiveAuthClient("000000004802B729", " ", null);
        }

        [TestMethod]
        public void LiveAuthClient_Constructor_Normal()
        {
            new LiveAuthClient("000000004802B729", "password", null);
        }

        [TestMethod]
        public void LiveAuthClient_GetLoginUrl_WithAllParams()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            Dictionary<string, string> options = new Dictionary<string,string>();
            options[AuthConstants.ClientState] = "unit tests";
            options[AuthConstants.Locale] = "zh-hans";
            options[AuthConstants.Display] = "touch";
            options["random"] = "random";
            string url = authClient.GetLoginUrl(new string[] { "wl.signin", "wl.basic" }, "https://www.foo.com", options);

            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=https%3A%2F%2Fwww.foo.com&scope=wl.signin%20wl.basic&response_type=code&display=touch&locale=zh-hans&state=appstate%3Dunit%2520tests", url);
        }
        
        [TestMethod]
        public void LiveAuthClient_GetLoginUrl_NoOptionalParameters()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            Dictionary<string, string> options = new Dictionary<string, string>();
            string url = authClient.GetLoginUrl(new string[] { "wl.signin", "wl.basic" }, "https://www.foo.com", null);

            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=https%3A%2F%2Fwww.foo.com&scope=wl.signin%20wl.basic&response_type=code&display=page&locale=en-US&state=", url);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_GetLoginUrl_WithInvalidDisplay()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            Dictionary<string, string> options = new Dictionary<string, string>();
            options[AuthConstants.ClientState] = "unit tests";
            options[AuthConstants.Locale] = "zh-hans";
            options[AuthConstants.Display] = "xbox";
            options["random"] = "random";
            string url = authClient.GetLoginUrl(new string[] { "wl.signin", "wl.basic" }, "https://www.foo.com", options);

            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=https%3A%2F%2Fwww.foo.com&scope=wl.signin%20wl.basic&response_type=code&display=touch&locale=zh-hans&state=appstate%3Dunit%2520tests", url);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveAuthClient_GetLoginUrl_WithNullScopes()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLoginUrl(null, "https://www.foo.com", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_GetLoginUrl_WithEmptyScopes()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLoginUrl(new string[] { }, "https://www.foo.com", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveAuthClient_GetLoginUrl_NullRedirectUrl()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLoginUrl(new string[] { "wl.signin" }, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_GetLoginUrl_WhiteSpaceRedirectUrl()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLoginUrl(new string[] { "wl.signin" }, " ", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveAuthClient_GetLoginUrl_InvalidSchemeRedirectUrl()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLoginUrl(new string[] { "wl.signin" }, "ftp://www.foo.com/callback", null);
        }

        [TestMethod]
        public void LiveAuthClient_GetLogoutUrl_Normal()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string url = authClient.GetLogoutUrl("http://www.foo.com/callback");
            Assert.AreEqual("https://login.live.com/oauth20_logout.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback", url);
         }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void LiveAuthClient_GetUserId_Empty()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string uid = authClient.GetUserId("");            
        }

        [TestMethod]
        [ExpectedException(typeof(LiveAuthException))]
        public void LiveAuthClient_GetUserId_Invalid()
        {
            LiveAuthClient authClient = new LiveAuthClient("000000004802B729", "password", null);
            string uid = authClient.GetUserId("etaeetwetw");
        }
    }
}
