namespace Microsoft.Live.UnitTests
{
    using System;

#if NETFX_CORE
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    [TestClass]
    public class LiveOperationProgressTest
    {
        // TODO: Test PrecentProgress
    }
}
