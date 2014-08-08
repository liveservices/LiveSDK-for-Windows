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
