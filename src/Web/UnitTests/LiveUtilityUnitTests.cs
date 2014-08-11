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
