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
