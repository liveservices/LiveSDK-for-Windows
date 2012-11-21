using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Live.Web.UnitTests
{
    [TestClass]
    public class LiveAuthWebUtilityUnitTests
    {
        [TestMethod]
        public void GetExpiresInString()
        {
            Assert.AreEqual("0", LiveAuthWebUtility.GetExpiresInString(DateTimeOffset.UtcNow));            
        }

        [TestMethod]
        public void ExpiresConversion()
        {
            DateTimeOffset expires = new DateTimeOffset(2012, 9, 19, 21, 23, 0, TimeSpan.FromSeconds(0));
            string expiresString = LiveAuthWebUtility.GetExpiresString(expires);

            Assert.AreEqual("1348089780", expiresString);
            Assert.AreEqual(expires, LiveAuthWebUtility.ParseExpiresValue(expiresString));
        }
    }
}
