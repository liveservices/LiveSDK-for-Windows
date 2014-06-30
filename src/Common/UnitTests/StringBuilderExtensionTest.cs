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
    using System.Text;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE || WEB || DESKTOP
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    [TestClass]
    public class StringBuilderExtensionTest
    {
        [TestMethod]
        public void TestAppendUrlPathStringEmpty()
        {
            var sb = new StringBuilder("/some/path");

            Assert.AreEqual(sb, sb.AppendUrlPath(string.Empty));

            Assert.AreEqual("/some/path/", sb.ToString());
        }

        [TestMethod]
        public void TestAppendUrlPathNull()
        {
            var sb = new StringBuilder("/some/path");

            Assert.AreEqual(sb, sb.AppendUrlPath(null));

            Assert.AreEqual("/some/path", sb.ToString());
        }

        [TestMethod]
        public void TestAppendUrlPathBasePathIsEmpty()
        {
            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendUrlPath("some/path"));

            Assert.AreEqual("some/path", sb.ToString());
        }

        [TestMethod]
        public void TestAppendUrlPathBasePathEndsInForwardSlashAppendedPathBeginsWithForwardSlash()
        {
            var sb = new StringBuilder("/some/");

            Assert.AreEqual(sb, sb.AppendUrlPath("/path"));

            Assert.AreEqual("/some/path", sb.ToString());
        }

        [TestMethod]
        public void TestAppendUrlPathBasePathDoesNotEndInForwardSlashAppendedPathBeginsWithForwardSlash()
        {
            var sb = new StringBuilder("/some");

            Assert.AreEqual(sb, sb.AppendUrlPath("/path"));

            Assert.AreEqual("/some/path", sb.ToString());
        }

        [TestMethod]
        public void TestAppendUrlPathBasePathDoesNotEndInForwardSlashAppendedPathDoesNotBeginWithForwardSlash()
        {
            var sb = new StringBuilder("/some");

            Assert.AreEqual(sb, sb.AppendUrlPath("path"));

            Assert.AreEqual("/some/path", sb.ToString());
        }

        [TestMethod]
        public void TestAppendQueryParameter()
        {
            var sb = new StringBuilder("/some/path?");

            Assert.AreEqual(sb, sb.AppendQueryParam("key", "value"));

            Assert.AreEqual("/some/path?key=value", sb.ToString());
        }
    }
}
