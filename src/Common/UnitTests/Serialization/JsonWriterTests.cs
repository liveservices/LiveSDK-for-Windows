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
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.IO;
    using System.Text;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE || DESKTOP || WEB
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif
    
    [TestClass]
    public class JsonWriterUnitTests
    {
        [TestMethod]
        public void JsonWriter_Normal()
        {
            var body = new Dictionary<string, object>();
            body.Add("foo", "bar");
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var jw = new JsonWriter(sw);
                jw.WriteValue(body);
                sw.Flush();
            }

            Assert.AreEqual("{\"foo\":\"bar\"}", sb.ToString());
        }

        [TestMethod]
        public void JsonWriter_EscapeCharacters()
        {
            var body = new Dictionary<string, object>();
            body.Add("b", "\b");
            body.Add("t", "\t");
            body.Add("n", "\n");
            body.Add("f", "\f");
            body.Add("r", "\r");
            body.Add("q", "\"");
            body.Add("rs", "\\");
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var jw = new JsonWriter(sw);
                jw.WriteValue(body);
                sw.Flush();
            }

            Assert.AreEqual("{" +
                @"""b"":""\u0008""," +
                @"""t"":""\u0009""," +
                @"""n"":""\u000a""," +
                @"""f"":""\u000c""," +
                @"""r"":""\u000d""," +
                @"""q"":""\""""," +
                @"""rs"":""\\""" + "}",
                sb.ToString());
        }

        [TestMethod]
        public void WriteValueNull()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(null);

            Assert.AreEqual("null", sb.ToString());
        }

        [TestMethod]
        public void WriteValueBoolFalse()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(false);

            Assert.AreEqual("false", sb.ToString());
        }

        [TestMethod]
        public void WriteValueBoolTrue()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(true);

            Assert.AreEqual("true", sb.ToString());
        }

        [TestMethod]
        public void WriteValueIntMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(int.MaxValue);

            Assert.AreEqual(int.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueIntMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(int.MinValue);

            Assert.AreEqual(int.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueUnsignedIntMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(uint.MaxValue);

            Assert.AreEqual(uint.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueUnsignedIntMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(uint.MinValue);

            Assert.AreEqual(uint.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueLongMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(long.MaxValue);

            Assert.AreEqual(long.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueLongMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(long.MinValue);

            Assert.AreEqual(long.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueUnsignedLongMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(ulong.MaxValue);

            Assert.AreEqual(ulong.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueByteMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(byte.MaxValue);

            Assert.AreEqual(byte.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueByteMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(byte.MinValue);

            Assert.AreEqual(byte.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueSignedByteMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(sbyte.MaxValue);

            Assert.AreEqual(sbyte.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueSignedByteMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(sbyte.MinValue);

            Assert.AreEqual(sbyte.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueShortMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(short.MaxValue);

            Assert.AreEqual(short.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueShortMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(short.MinValue);

            Assert.AreEqual(short.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueUnsignedShortMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(ushort.MaxValue);

            Assert.AreEqual(ushort.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueUnsignedShortMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(ushort.MinValue);

            Assert.AreEqual(ushort.MinValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueFloatMax()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(float.MaxValue);

            Assert.AreEqual(float.MaxValue.ToString(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueFloatMin()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(float.MinValue);

            Assert.AreEqual(float.MinValue.ToString(), sb.ToString());
        }

        private enum TestEnum
        {
            Foo
        }

        [TestMethod]
        public void WriteValueEnum()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            jsonWriter.WriteValue(TestEnum.Foo);

            Assert.AreEqual('"' + TestEnum.Foo.ToString() + '"', sb.ToString());
        }

        [TestMethod]
        public void WriteValueDateTime()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var dt = new DateTime(1970, 1, 1, 0, 0, 30, DateTimeKind.Utc);
            jsonWriter.WriteValue(dt);

            Assert.AreEqual('"' + (30 * 1000).ToString() + '"', sb.ToString());
        }

        [TestMethod]
        public void WriteValueGuid()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var dt = Guid.NewGuid();
            jsonWriter.WriteValue(dt);

            Assert.AreEqual('"' + dt.ToString() + '"', sb.ToString());
        }

        private class TestIJsonSerializable : IJsonSerializable
        {
            public string ToJson()
            {
                return "TestIJsonSerializable";
            }
        }

        [TestMethod]
        public void WriteValueIJsonSerializable()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var test = new TestIJsonSerializable();
            jsonWriter.WriteValue(test);

            Assert.AreEqual(test.ToJson(), sb.ToString());
        }

        [TestMethod]
        public void WriteValueIDictionaryEmpty()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var dictionary = new Dictionary<string, object>();
            jsonWriter.WriteValue(dictionary);

            Assert.AreEqual("{}", sb.ToString());
        }

        [TestMethod]
        public void WriteValueIDictionary()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var dictionary = new Dictionary<string, object>();
            dictionary["foo"] = "bar";
            jsonWriter.WriteValue(dictionary);

            Assert.AreEqual("{\"foo\":\"bar\"}", sb.ToString());
        }

        [TestMethod]
        public void WriteValueICollectionEmpty()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var collection = new Collection<object>();
            jsonWriter.WriteValue(collection);

            Assert.AreEqual("[]", sb.ToString());
        }

        [TestMethod]
        public void WriteValueICollection()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var jsonWriter = new JsonWriter(stringWriter);

            var collection = new Collection<object>();
            collection.Add("foo");
            collection.Add(true);
            collection.Add(int.MaxValue);
            jsonWriter.WriteValue(collection);

            Assert.AreEqual("[\"foo\",true," + int.MaxValue + "]", sb.ToString());
        }
    }
}
