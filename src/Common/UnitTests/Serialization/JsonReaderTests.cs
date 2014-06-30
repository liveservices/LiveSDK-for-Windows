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

namespace Microsoft.Live.Serialization.UnitTests
{
    using System;
    using System.Collections.Generic;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE || DESKTOP || WEB
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    [TestClass]
    public class JsonReaderTests
    {
        [TestMethod]
        public void JsonReader_Normal()
        {  
            string json = @"{ name: ""foo"",
description: ""hello me""}";

            using (var jsonReader = new JsonReader(json, false))
            {
                dynamic requestBodyJson = (IDictionary<string, object>)jsonReader.ReadValue();

                Assert.AreEqual("foo", requestBodyJson.name);

                Assert.AreEqual("hello me", requestBodyJson.description);
            }
        }

        [TestMethod]
        public void JsonReader_EscapedCharacters()
        {
            string json = "{" +
                "\"b\":\"\\b\"," +
                "\"t\":\"\\t\"," +
                "\"n\":\"\\n\"," +
                "\"f\":\"\\f\"," +
                "\"r\":\"\\r\"," +
                "\"q\":\"\\\"\"," +
                "\"rs\":\"\\\\\"" + "}";

            using (var jsonReader = new JsonReader(json, false))
            {
                dynamic requestBodyJson = (IDictionary<string, object>)jsonReader.ReadValue();

                Assert.AreEqual("\b", requestBodyJson.b);
                Assert.AreEqual("\t", requestBodyJson.t);
                Assert.AreEqual("\n", requestBodyJson.n);
                Assert.AreEqual("\f", requestBodyJson.f);
                Assert.AreEqual("\r", requestBodyJson.r);
                Assert.AreEqual("\"", requestBodyJson.q);
                Assert.AreEqual("\\", requestBodyJson.rs);
            }
        }

        [TestMethod]
        public void JsonReader_InvalidFormat()
        {
            string json = "{ name: \"";

            using (var jsonReader = new JsonReader(json, false))
            {
                try
                {
                    jsonReader.ReadValue();
                    Assert.Fail("Expected FormatException to be thrown.");
                }
                catch (FormatException)
                {
                }
            }
        }

        [TestMethod]
        public void TestReadValueObjectEmpty()
        {
            var jsonReader = new JsonReader("{}");

            object value = jsonReader.ReadValue();

            Assert.IsTrue(
                value is DynamicDictionary,
                "Value read is not a DynamicDictionary");

            var dictionary = value as DynamicDictionary;

            Assert.AreEqual(0, dictionary.Count);
        }

        [TestMethod]
        public void TestReadValueArrayEmpty()
        {
            var jsonReader = new JsonReader("[]");

            object value = jsonReader.ReadValue();

            Assert.IsTrue(
                value is List<object>,
                "Value read is not a List<object>");

            var list = value as List<object>;

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void TestReadValueStringEmpty()
        {
            var jsonReader = new JsonReader("\"\"");

            object value = jsonReader.ReadValue();

            Assert.IsTrue(
                value is string,
                "Value read is not a string");

            Assert.AreEqual(string.Empty, value as string);
        }

        [TestMethod]
        public void TestReadValueString()
        {
            var jsonReader = new JsonReader("\"foo\"");

            object value = jsonReader.ReadValue();

            Assert.IsTrue(
                value is string,
                "Value read is not a string");

            Assert.AreEqual("foo", value as string);
        }

        [TestMethod]
        public void TestReadValueInteger32BitPositive()
        {
            var jsonReader = new JsonReader("{ \"data\": " + int.MaxValue.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is int,
                "Value read is not a int");

            Assert.AreEqual(int.MaxValue, (int)value.data);
        }

        [TestMethod]
        public void TestReadValueInteger32BitNegative()
        {
            var jsonReader = new JsonReader("{ \"data\": " + int.MinValue.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is int,
                "Value read is not a string");

            Assert.AreEqual(int.MinValue, (int)value.data);
        }

        [TestMethod]
        public void TestReadValueInteger64BitPositive()
        {
            var jsonReader = new JsonReader("{ \"data\": " + long.MaxValue.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is long,
                "Value read is not a long");

            Assert.AreEqual(long.MaxValue, (long)value.data);
        }

        [TestMethod]
        public void TestReadValueInteger64BitNegative()
        {
            var jsonReader = new JsonReader("{ \"data\": " + long.MinValue.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is long,
                "Value read is not a long");

            Assert.AreEqual(long.MinValue, (long)value.data);
        }

        [TestMethod]
        public void TestReadValueIntegerInvalidPositive()
        {
            var jsonReader = new JsonReader(long.MaxValue.ToString() + "0");

            try
            {
                jsonReader.ReadValue();
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }
        }

        [TestMethod]
        public void TestReadValueIntegerInvalidNegative()
        {
            var jsonReader = new JsonReader(long.MinValue.ToString() + "0");

            try
            {
                jsonReader.ReadValue();
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }
        }

        [TestMethod]
        public void TestReadValueFloat()
        {
            var jsonReader = new JsonReader("{ \"data\": " + 0.5f.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is float,
                "Value read is not a float");

            Assert.AreEqual(0.5f, (float)value.data);
        }

        [TestMethod]
        public void TestReadValueFloatWithExpentionalNotationPositive()
        {
            // ToString and Back loses some fidelity so we do it now
            // to be able to compare the return value from ReadValue
            float max = float.Parse(float.MaxValue.ToString());
            var jsonReader = new JsonReader("{ \"data\": " + max.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is float,
                "Value read is not a float");

            Assert.AreEqual(max, (float)value.data);
        }

        [TestMethod]
        public void TestReadValueFloatWithExpentionalNotationNegative()
        {
            // ToString and Back loses some fidelity so we do it now
            // to be able to compare the return value from ReadValue
            float min = float.Parse(float.MinValue.ToString());
            var jsonReader = new JsonReader("{ \"data\": " + min.ToString() + " }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is float,
                "Value read is not a float");

            Assert.AreEqual(min, (float)value.data);
        }

        [TestMethod]
        public void TestReadValueBooleanTrue()
        {
            var jsonReader = new JsonReader("{ \"data\": true }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is bool,
                "Value read is not a bool");

            Assert.AreEqual(true, (bool)value.data);
        }

        [TestMethod]
        public void TestReadValueBooleanFalse()
        {
            var jsonReader = new JsonReader("{ \"data\": false }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsTrue(
                value.data is bool,
                "Value read is not a bool");

            Assert.AreEqual(false, (bool)value.data);
        }

        [TestMethod]
        public void TestReadValueNull()
        {
            var jsonReader = new JsonReader("{ \"data\": null }");

            dynamic value = jsonReader.ReadValue();

            Assert.IsNull(value.data, "Expected Value read to be null");
        }

        [TestMethod]
        public void TestReadInvalidJsonTextEmptyString()
        {
            var jsonReader = new JsonReader("");

            try
            {
                jsonReader.ReadValue();
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }
        }

        [TestMethod]
        public void TestReadInvalidJsonText()
        {
            var jsonReader = new JsonReader("klasdjfas89123;kja#!");

            try
            {
                jsonReader.ReadValue();
                Assert.Fail("Expected FormatException to be thrown.");
            }
            catch (FormatException)
            {
            }
        }
    }
}
