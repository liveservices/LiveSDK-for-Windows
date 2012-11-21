using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Live;

namespace Microsoft.Live.Web.UnitTests
{
    [TestClass]
    public class LiveUtilityUnitTests
    {
        [TestMethod]
        public void LiveUtility_ValidateNotNullParameter_Normal()
        {
            LiveUtility.ValidateNotNullParameter("4303432830243", "client_id");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveUtility_ValidateNotNullParameter_Invalid()
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(null, "client_id");
        }

        [TestMethod]
        public void LiveUtility_ValidateNotNullOrEmptyString_Normal()
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString("4303432830243", "client_id");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveUtility_ValidateNotNullOrEmptyString_NullParameter()
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(null, "client_id");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveUtility_ValidateNotNullOrEmptyString_EmptyParameter()
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString("", "client_id");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveUtility_ValidateNotNullOrEmptyString_WhiteSpaceParameter()
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(" \t  ", "client_id");
        }

        [TestMethod]
        public void LiveUtility_ValidateUrl_http()
        {
            LiveUtility.ValidateUrl("http://www.foo.com", "redirectUrl");
        }

        [TestMethod]
        public void LiveUtility_ValidateUrl_https()
        {
            LiveUtility.ValidateUrl("https://www.foo.com", "redirectUrl");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LiveUtility_ValidateUrl_Null()
        {
            LiveUtility.ValidateUrl(null, "redirectUrl");
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentException))]
        public void LiveUtility_ValidateUrl_WhiteSpace()
        {
            LiveUtility.ValidateUrl("  ", "redirectUrl");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiveUtility_ValidateUrl_InvalidScheme()
        {
            LiveUtility.ValidateUrl("ftp://foo.com/callback", "redirectUrl");
        }
    }
}
