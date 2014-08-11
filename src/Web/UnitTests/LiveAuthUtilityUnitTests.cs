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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Live.Web.UnitTests
{
    [TestClass]
    public class LiveAuthUtilityUnitTests
    {
        [TestInitialize]
        public void TestInit()
        {
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            LiveAuthClient.AuthEndpointOverride = null;
        }

        [TestMethod]
        public void LiveAuthUtility_BuildScopeString_SingleScope()
        {
            string[] scopes = new string[] { "wl.signin" };
            string scopeStr = LiveAuthUtility.BuildScopeString(scopes);
            Assert.AreEqual("wl.signin", scopeStr);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildScopeString_MultipleScopes()
        {
            string[] scopes = new string[] { "wl.signin", "wl.basic", "wl.contacts" };
            string scopeStr = LiveAuthUtility.BuildScopeString(scopes);
            Assert.AreEqual("wl.signin wl.basic wl.contacts", scopeStr);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildScopeString_EmptyScopes()
        {
            string[] scopes = new string[] {  };
            string scopeStr = LiveAuthUtility.BuildScopeString(scopes);
            Assert.AreEqual(string.Empty, scopeStr);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildScopeString_NullScopes()
        {
            string scopeStr = LiveAuthUtility.BuildScopeString(null);
            Assert.AreEqual(string.Empty, scopeStr);
        }

        [TestMethod]
        public void LiveAuthUtility_ParseScopeString_Empty()
        {
            object scopes = LiveAuthUtility.ParseScopeString("");
            List<string> scopeList = (List<string>)scopes;
            Assert.AreEqual(0, scopeList.Count);
        }

        [TestMethod]
        public void LiveAuthUtility_ParseScopeString_SpaceDelimited()
        {
            object scopes = LiveAuthUtility.ParseScopeString("wl.signin wl.basic wl.skydrive");
            List<string> scopeList = (List<string>)scopes;
            Assert.AreEqual(3, scopeList.Count);
            Assert.AreEqual("wl.signin", scopeList[0]);
            Assert.AreEqual("wl.basic", scopeList[1]);
            Assert.AreEqual("wl.skydrive", scopeList[2]);
        }
                        
        [TestMethod]
        public void LiveAuthUtility_ParseScopeString_CommaDelimited()
        {
            object scopes = LiveAuthUtility.ParseScopeString("wl.signin,wl.basic,wl.skydrive ");
            List<string> scopeList = (List<string>)scopes;
            Assert.AreEqual(3, scopeList.Count);
            Assert.AreEqual("wl.signin", scopeList[0]);
            Assert.AreEqual("wl.basic", scopeList[1]);
            Assert.AreEqual("wl.skydrive", scopeList[2]);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildLogoutUrl()
        {
            string url = LiveAuthUtility.BuildLogoutUrl("000000004802B729", "http://www.foo.com/callback.aspx");
            Assert.AreEqual("https://login.live.com/oauth20_logout.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx", url);            
        }

        [TestMethod]
        public void LiveAuthUtility_BuildLogoutUrl_WithHostOverride()
        {
            LiveAuthClient.AuthEndpointOverride = "https://login.live-int.com";
            string url = LiveAuthUtility.BuildLogoutUrl("000000004802B729", "http://www.foo.com/callback.aspx");
            Assert.AreEqual("https://login.live-int.com/oauth20_logout.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildAuthorizeUrl_WithTheme()
        {
            string url = LiveAuthUtility.BuildAuthorizeUrl(
                "000000004802B729", 
                "http://www.foo.com/callback.aspx",
                new string[]{"wl.signin", "wl.basic"},
                ResponseType.Code,
                DisplayType.WinDesktop,
                ThemeType.Win7,
                "en-US",
                "login page");
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=windesktop&locale=en-US&state=appstate%3Dlogin%2520page&theme=win7", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildAuthorizeUrl_WithNoTheme()
        {
            string url = LiveAuthUtility.BuildAuthorizeUrl(
                "000000004802B729",
                "http://www.foo.com/callback.aspx",
                new string[] { "wl.signin", "wl.basic" },
                ResponseType.Code,
                DisplayType.WinDesktop,
                ThemeType.None,
                "en-US",
                "login page");
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=windesktop&locale=en-US&state=appstate%3Dlogin%2520page", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildAuthorizeUrl_WithNoState()
        {
            string url = LiveAuthUtility.BuildAuthorizeUrl(
                "000000004802B729",
                "http://www.foo.com/callback.aspx",
                new string[] { "wl.signin", "wl.basic" },
                ResponseType.Code,
                DisplayType.WinDesktop,
                ThemeType.Win7,
                "en-US",
                null);
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=windesktop&locale=en-US&state=&theme=win7", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildAuthorizeUrl_WithEmptyState()
        {
            string url = LiveAuthUtility.BuildAuthorizeUrl(
                "000000004802B729",
                "http://www.foo.com/callback.aspx",
                new string[] { "wl.signin", "wl.basic" },
                ResponseType.Code,
                DisplayType.WinDesktop,
                ThemeType.Win7,
                "en-US",
                string.Empty);
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=windesktop&locale=en-US&state=appstate%3D&theme=win7", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildAuthorizeUrl_WithHostOverride()
        {
            LiveAuthClient.AuthEndpointOverride = "https://login.live-int.com";
            string url = LiveAuthUtility.BuildAuthorizeUrl(
                "000000004802B729",
                "http://www.foo.com/callback.aspx",
                new string[] { "wl.signin", "wl.basic" },
                ResponseType.Code,
                DisplayType.WinDesktop,
                ThemeType.Win7,
                "en-US",
                "login page");
            Assert.AreEqual("https://login.live-int.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=windesktop&locale=en-US&state=appstate%3Dlogin%2520page&theme=win7", url);
        }

        [TestMethod]
        public void LiveAuthUtility_BuildWebAuthorizeUrl()
        {
            string url = LiveAuthUtility.BuildWebAuthorizeUrl(
                "000000004802B729",
                "http://www.foo.com/callback.aspx",
                new string[] { "wl.signin", "wl.basic" },
                DisplayType.Page,
                "zh-hans",
                "login page");
            Assert.AreEqual("https://login.live.com/oauth20_authorize.srf?client_id=000000004802B729&redirect_uri=http%3A%2F%2Fwww.foo.com%2Fcallback.aspx&scope=wl.signin%20wl.basic&response_type=code&display=page&locale=zh-hans&state=appstate%3Dlogin%2520page", url);
        }

        [TestMethod]
        public void LiveAuthUtility_GetCurrentRedirectUrl_NoQeury()
        {
            Uri uri = new Uri("http://www.foo.com/callback.aspx");
            Assert.AreEqual("http://www.foo.com/callback.aspx", LiveAuthUtility.GetCurrentRedirectUrl(uri));
        }

        [TestMethod]
        public void LiveAuthUtility_GetCurrentRedirectUrl_QueryWithoutAuthParams()
        {
            Uri uri = new Uri("http://www.foo.com/callback.aspx?a=b&c=d");
            Assert.AreEqual("http://www.foo.com/callback.aspx?a=b&c=d", LiveAuthUtility.GetCurrentRedirectUrl(uri));
        }

        [TestMethod]
        public void LiveAuthUtility_GetCurrentRedirectUrl_QueryWithOnlyAuthParams()
        {
            Uri uri = new Uri("http://www.foo.com/callback.aspx?code=4342332334&state=appstate%25hellome");
            Assert.AreEqual("http://www.foo.com/callback.aspx", LiveAuthUtility.GetCurrentRedirectUrl(uri));
        }

        [TestMethod]
        public void LiveAuthUtility_GetCurrentRedirectUrl_QueryWithMixedAuthAndOtherParams()
        {
            Uri uri = new Uri("http://www.foo.com/callback.aspx?a=b&c=d&code=4342332334&state=appstate%25hellome");
            Assert.AreEqual("http://www.foo.com/callback.aspx?a=b&c=d", LiveAuthUtility.GetCurrentRedirectUrl(uri));
        }

        [TestMethod]
        public void LiveAuthUtility_ReadToken()
        {
            string authToken = "eyJhbGciOiJIUzI1NiIsImtpZCI6IjAiLCJ0eXAiOiJKV1QifQ.eyJ2ZXIiOjEsImlzcyI6InVybjp3aW5kb3dzOmxpdmVpZCIsImV4cCI6MTM0NzczNzI0NSwidWlkIjoiYjY5ZTllOTlkYzE4MzE1YmZkOWJmOWJjY2Y4ZGY2NjkiLCJhdWQiOiJ3d3cubGl2ZXNka2phdmFzY3JpcHRzYW1wbGVzLmNvbSIsInVybjptaWNyb3NvZnQ6YXBwdXJpIjoibXMtYXBwOi8vUy0xLTE1LTItMTI4MjA3MDczMC0yMzQyMTY3OTUyLTI3NjQyOTkxMTgtMjcwOTk1MTQxNi0yMDQ2NDEyNTctMzM0NTQ3NjMxOS0xMDE1MzA5MDI0IiwidXJuOm1pY3Jvc29mdDphcHBpZCI6IjAwMDAwMDAwNDAwODYyMUUifQ.c6ypLLB3MXD5gRj3M09lJSCKJNfGA8hw7ykEE5aF0OU";
            string secret = "Gzy5MgBnMol73vTWZ3lqB34Pltt1XQiQ";
            JsonWebToken token;
            LiveAuthException error;
            bool succeeded = LiveAuthWebUtility.DecodeAuthenticationToken(authToken, secret, out token, out error);

            Assert.IsTrue(succeeded);
            Assert.AreEqual("b69e9e99dc18315bfd9bf9bccf8df669", token.Claims.UserId);
            Assert.IsTrue(token.IsExpired);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void LiveAuthUtility_EncodeAppRequestState()
        {
            Assert.AreEqual("appstate=hello%20me", LiveAuthUtility.EncodeAppRequestState("hello me"));
        }

        [TestMethod]
        public void LiveAuthUtility_DecodeAppRequestState()
        {
            Assert.AreEqual("hello me", LiveAuthUtility.DecodeAppRequestStates("appstate=hello%20me")[AuthConstants.AppState]);
        }

        [TestMethod]
        public void LiveAuthUtility_IsSubSetScopesRange()
        {
            Assert.IsTrue(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.signin", "wl.basic" }, new string[] { "wl.signin", "wl.basic" }));
            Assert.IsTrue(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.signin", "wl.basic" }, new string[] { "wl.basic", "wl.signin" }));
            Assert.IsTrue(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.signin", "wl.basic" }, new string[] { "wl.basic", "wl.signin", "wl.skydrive" }));
            Assert.IsTrue(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.signin", "wl.basic" }, new string[] { "wl.skydrive", "wl.basic", "wl.signin" }));
            Assert.IsTrue(LiveAuthUtility.IsSubsetOfScopeRange(null, new string[] { "wl.basic", "wl.signin", "wl.skydrive" }));

            Assert.IsFalse(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.calendar" }, new string[] { "wl.basic", "wl.signin", "wl.skydrive" }));
            Assert.IsFalse(LiveAuthUtility.IsSubsetOfScopeRange(new string[] { "wl.signin", "wl.basic", "wl.calendar" }, new string[] { "wl.basic", "wl.signin", "wl.skydrive" }));
        }
    }
}
