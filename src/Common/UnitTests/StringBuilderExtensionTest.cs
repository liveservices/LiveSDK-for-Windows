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
