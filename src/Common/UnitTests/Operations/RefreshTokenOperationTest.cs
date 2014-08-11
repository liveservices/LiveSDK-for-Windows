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

namespace Microsoft.Live.UnitTests
{
    using System;
    using System.Collections.Generic;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;

    [TestClass]
    public class RefreshTokenOperationTest
    {
        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();

#if NETFX_CORE
            LiveAuthClient authClient = new LiveAuthClient();
#elif WINDOWS_PHONE
            LiveAuthClient authClient = new LiveAuthClient("clientId");
#else
#error This platform needs to be handled.
#endif

            string clientId = "clientId";
            string refreshToken = "refreshToken";
            IEnumerable<string> scopes = new string[]{ "wl.basic" };
            SynchronizationContextWrapper syncContext = SynchronizationContextWrapper.Current;

            var refreshOperation =
                new RefreshTokenOperation(authClient, clientId, refreshToken, scopes, syncContext);
        }
    }
}
