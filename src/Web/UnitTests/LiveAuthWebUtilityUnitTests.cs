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
