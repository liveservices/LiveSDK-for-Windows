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

namespace Microsoft.Live.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE || WEB || DESKTOP
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    [TestClass]
    public class DynamicDictionaryTest
    {
        #region DynamicObject tests

        [TestMethod]
        public void TestTryGetMemberKeyExists()
        {
            var dynamicDictionary = new DynamicDictionary();
            dynamicDictionary["foo"] = "bar";

            dynamic d = dynamicDictionary;

            Assert.AreEqual("bar", d.foo);
        }

        [TestMethod]
        public void TestTryGetMemberKeyDoesNotExists()
        {
            var dynamicDictionary = new DynamicDictionary();

            dynamic d = dynamicDictionary;

            Assert.IsNull(d.foo);
        }

        [TestMethod]
        public void TestTrySetMemberKeyDoesNotExistYet()
        {
            var dynamicDictionary = new DynamicDictionary();

            dynamic d = dynamicDictionary;

            d.foo = "bar";

            Assert.AreEqual("bar", d.foo);
        }

        [TestMethod]
        public void TestTrySetMemberKeyAlreadyExists()
        {
            var dynamicDictionary = new DynamicDictionary();

            dynamic d = dynamicDictionary;

            d["foo"] = "bar";
            d.foo = "bar2";

            Assert.AreEqual("bar2", d.foo);
        }

        #endregion

        #region IDictionary<string, object> tests

        [TestMethod]
        public void TestAdd()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            Assert.AreEqual("value", d["key"]);
        }

        [TestMethod]
        public void TestContainsKey()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            Assert.IsTrue(d.ContainsKey("key"), "Did not contain key: \"key\"");
        }

        [TestMethod]
        public void TestRemove()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            Assert.IsTrue(d.Remove("key"), "Did not remove key: \"key\"");
        }

        [TestMethod]
        public void TestTryGetValueKeyExists()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            object value;

            Assert.IsTrue(
                d.TryGetValue("key", out value), 
                "Expected true to trying to get a value for an existing key.");

            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void TestTryGetValueKeyDoesNotExist()
        {
            var d = new DynamicDictionary();

            object value;

            Assert.IsFalse(
                d.TryGetValue("key", out value),
                "Expected false to trying to get a value for a non exisiting key.");

            Assert.IsNull(
                value, 
                "Retrieved a Non null value for a key that doesn't exist.");
        }

        [TestMethod]
        public void TestValuesFromEmptyDynamicDictionary()
        {
            var d = new DynamicDictionary();

            ICollection<object> values = d.Values;

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void TestValuesFromEmptyDynamicDictionaryThatOnceHadItemsInIt()
        {
            var d = new DynamicDictionary();

            ICollection<object> values = d.Values;

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void TestValuesFromNonEmptyDynamicDictionary()
        {
            var d = new DynamicDictionary();

            d["foo"] = "bar";


            ICollection<object> values = d.Values;

            Assert.AreEqual(1, values.Count);

            foreach (object value in values)
            {
                Assert.AreEqual("bar", value);
            }
        }

        [TestMethod]
        public void TestValuesSeeIfModifyingTheReturnValuesAffectsTheOriginalDynamicDictionary()
        {
            var d = new DynamicDictionary();

            d["foo"] = "bar";


            ICollection<object> values = d.Values;

            try
            {
                values.Clear();
                Assert.Fail("Did not throw a NotSupportedException. The values collection was allowed to be modified.");
            }
            catch (NotSupportedException)
            {
            }

            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(1, d.Count);
        }

        [TestMethod]
        public void TestSquareBacesOperator()
        {
            var d = new DynamicDictionary();

            d["foo"] = "bar";

            Assert.AreEqual("bar", d["foo"]);
        }

        [TestMethod]
        public void TestSquareBacesOperatorKeyDoesNotExist()
        {
            var d = new DynamicDictionary();

            try
            {
                object value = d["foo"];
                Assert.Fail("Did not receive KeyNotFoundException for a key that doesn't exist.");
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void TestAddKeyValuePair()
        {
            var d = new DynamicDictionary();

            d.Add(new KeyValuePair<string, object>("foo", "bar"));

            Assert.AreEqual("bar", d["foo"]);
        }

        [TestMethod]
        public void TestClearNonEmptyList()
        {
            var d = new DynamicDictionary();

            d.Add(new KeyValuePair<string, object>("foo", "bar"));

            d.Clear();

            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        public void TestClearEmptyList()
        {
            var d = new DynamicDictionary();

            d.Clear();
            
            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        public void TestContainsKeyValuePairThatExists()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            Assert.IsTrue(d.Contains(new KeyValuePair<string, object>("key", "value")));
        }

        [TestMethod]
        public void TestContainsKeyValuePairThatDoesNotExists()
        {
            var d = new DynamicDictionary();

            d.Add("foo", "bar");

            Assert.IsFalse(d.Contains(new KeyValuePair<string, object>("key", "value")));
        }

        [TestMethod]
        public void TestCopyToKeyValuePair()
        {
            var d = new DynamicDictionary();

            d.Add("foo", "bar");

            var array = new KeyValuePair<string, object>[d.Count];

            d.CopyTo(array, 0);

            KeyValuePair<string, object> pair = array[0];

            Assert.AreEqual("foo", pair.Key);
            Assert.AreEqual("bar", pair.Value);
        }

        [TestMethod]
        public void TestCount()
        {
            var d = new DynamicDictionary();

            Assert.AreEqual(0, d.Count);

            d.Add("key", "value");

            Assert.AreEqual(1, d.Count);

            d.Remove("key");

            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        public void TestIsReadOnly()
        {
            var d = new DynamicDictionary();

            Assert.IsFalse(d.IsReadOnly);
        }

        [TestMethod]
        public void TestRemoveKeyValuePair()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            Assert.IsTrue(d.Remove(new KeyValuePair<string, object>("key", "value")));

            Assert.AreEqual(0, d.Count);
        }

        [TestMethod]
        public void TestGetEnumerator()
        {
            var d = new DynamicDictionary();

            d.Add("key", "value");

            IEnumerator enumerator = d.GetEnumerator();

            int count = 0;
            while (enumerator.MoveNext())
            {
                var pair = (KeyValuePair<string, object>) enumerator.Current;

                Assert.AreEqual("key", pair.Key);
                Assert.AreEqual("value", pair.Value);

                count += 1;
            }

            Assert.AreEqual(1, count, "Enumerator did not move through the right number of items.");
        }

        #endregion
    }
}
