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
