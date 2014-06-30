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

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE || WEB || DESKTOP
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    [TestClass]
    public class QueryParametersTest
    {
        [TestMethod]
        public void GetOverwriteParameterValueOverwrite()
        {
            Assert.AreEqual("true", QueryParameters.GetOverwriteValue(OverwriteOption.Overwrite));
        }

        [TestMethod]
        public void GetOverwriteParameterValueDoNotOverwrite()
        {
            Assert.AreEqual("false", QueryParameters.GetOverwriteValue(OverwriteOption.DoNotOverwrite));
        }

        [TestMethod]
        public void GetOverwriteParameterValueRename()
        {
            Assert.AreEqual("choosenewname", QueryParameters.GetOverwriteValue(OverwriteOption.Rename));
        }
    }
}
