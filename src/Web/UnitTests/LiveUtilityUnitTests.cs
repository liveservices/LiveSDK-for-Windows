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
