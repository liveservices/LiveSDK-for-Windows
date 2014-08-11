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

namespace Microsoft.Live.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    ///     Responsible for serializing to JSON.
    /// </summary>
    internal class JsonWriter
    {
        /// <summary>
        ///     The JSON escapes of the first 256 characters for quick lookup.
        /// </summary>
        private static readonly string[] QuotedCharacterMap;

        /// <summary>
        ///     Array indicating which characters should be encoded.
        /// </summary>
        private static readonly bool[] QuotedCharacters;

        /// <summary>
        ///     Array indicating which characters should be encoded if escaping control characters is disabled.
        /// </summary>
        private static readonly bool[] QuotedCharactersWithoutControlCharacters;

        /// <summary>
        ///     Epoch expressed as a DateTime.
        /// </summary>
        private static readonly DateTime Epoch;

        /// <summary>
        ///     The writer object to serialize to.
        /// </summary>
        private readonly IndentedTextWriter writer;

        /// <summary>
        ///     The currently running object scopes.
        /// </summary>
        private readonly Stack<Scope> scopes;

        /// <summary>
        ///     The quoted character map this instance should use based on the creator's desired behavior.
        /// </summary>
        private readonly bool[] quotedCharacters;

        /// <summary>
        ///     Initializes the JsonWriter class.
        /// </summary>
        static JsonWriter()
        {
            // build a list of quoted characters.
            string[] characterMap = new string[256];
            bool[] characters = new bool[256];
            bool[] charactersWithoutControl = new bool[256];

            for (int i = 0; i < 256; i++)
            {
                characterMap[i] = @"\u" + i.ToString("x", CultureInfo.InvariantCulture).PadLeft(4, '0');
            }

            characterMap[0x22] = @"\""";
            characterMap[0x5c] = @"\\";

            for (int i = 0; i < 256; i++)
            {
                characters[i] =
                    (i < 0x20) ||   // Control characters
                    (i == 0x22) ||  // Quotation mark '"'
                    (i == 0x5c);    // Reverse solidus '\'

                charactersWithoutControl[i] =
                    (i == 0x22) ||  // Quotation mark '"'
                    (i == 0x5c);    // Reverse solidus '\'
            }

            JsonWriter.QuotedCharacterMap = characterMap;
            JsonWriter.QuotedCharacters = characters;
            JsonWriter.QuotedCharactersWithoutControlCharacters = charactersWithoutControl;

            JsonWriter.Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        ///     Initializes a new instance of the JsonWriter class.
        /// </summary>
        public JsonWriter(TextWriter writer)
            : this(writer, true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the JsonWriter class.
        /// </summary>
        public JsonWriter(TextWriter writer, bool minimized)
            : this(writer, minimized, true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the JsonWriter class.
        /// </summary>
        public JsonWriter(TextWriter writer, bool minimized, bool escapeControlCharacters)
        {
            this.writer = new IndentedTextWriter(writer, minimized);
            this.scopes = new Stack<Scope>();
            this.quotedCharacters = escapeControlCharacters ? JsonWriter.QuotedCharacters : JsonWriter.QuotedCharactersWithoutControlCharacters;
        }

        private enum ScopeType
        {
            Array = 0,

            Object = 1,
        }

        /// <summary>
        ///     Writes a value.
        /// </summary>
        public void WriteValue(object o)
        {
            if (o == null)
            {
                this.WriteCore("null", false);
            }
            else if (o is bool)
            {
                this.WriteValue((bool)o);
            }
            else if (o is int)
            {
                this.WriteValue((int)o);
            }
            else if (o is long)
            {
                this.WriteValue((long)o);
            }
            else if (o is uint)
            {
                this.WriteValue((uint)o);
            }
            else if (o is ulong)
            {
                this.WriteValue((ulong)o);
            }
            else if (o is byte)
            {
                this.WriteValue((byte)o);
            }
            else if (o is sbyte)
            {
                this.WriteValue((sbyte)o);
            }
            else if (o is short)
            {
                this.WriteValue((short)o);
            }
            else if (o is ushort)
            {
                this.WriteValue((ushort)o);
            }
            else if (o is Enum)
            {
                this.WriteValue((Enum)o);
            }
            else if (o is float)
            {
                this.WriteValue((float)o);
            }
            else if (o is DateTime)
            {
                this.WriteValue((DateTime)o);
            }
            else if (o is string)
            {
                this.WriteValue((string)o);
            }
            else if (o is Guid)
            {
                this.WriteValue((Guid)o);
            }
            else if (o is IJsonSerializable)
            {
                this.WriteValue((IJsonSerializable)o);
            }
            else if (o is IDictionary<string, object>)
            {
                this.WriteValue((IDictionary<string, object>)o);
            }
            else if (o is ICollection)
            {
                this.WriteValue((ICollection)o);
            }
            else
            {
                throw new InvalidOperationException("unsupported type");
            }
        }

        protected virtual void WriteProperty(string name, object value)
        {
            this.WriteName(name);
            this.WriteValue(value);
        }

        protected virtual string NormalizeName(string name)
        {
            return name;
        }

        protected void WriteName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (this.scopes.Count == 0)
            {
                throw new InvalidOperationException("No active scope to write into.");
            }

            Scope currentScope = this.scopes.Peek();

            if (currentScope.Type != ScopeType.Object)
            {
                throw new InvalidOperationException("Names can only be written into Object scopes.");
            }

            if (currentScope.ObjectCount != 0)
            {
                this.writer.WriteTrimmed(", ");
                this.writer.WriteLine();
            }

            currentScope.ObjectCount++;

            this.WriteCore(this.NormalizeName(name), true);

            this.writer.WriteTrimmed(": ");
        }

        protected virtual void WriteValue(byte value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(sbyte value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(short value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(ushort value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(Enum value)
        {
            this.WriteCore(value.ToString(), true);
        }

        protected virtual void WriteValue(bool value)
        {
            this.WriteCore(value ? "true" : "false", false);
        }

        protected virtual void WriteValue(int value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(uint value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(long value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(ulong value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(float value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), false);
        }

        protected virtual void WriteValue(DateTime value)
        {
            this.WriteCore(Convert.ToInt64(value.ToUniversalTime().Subtract(JsonWriter.Epoch).TotalMilliseconds).ToString(CultureInfo.InvariantCulture), true);
        }

        protected virtual void WriteValue(string s)
        {
            if (s == null)
            {
                this.WriteCore("null", false);
            }
            else
            {
                this.WriteCore(this.EscapeString(s), true);
            }
        }

        protected virtual void WriteValue(Guid value)
        {
            this.WriteCore(value.ToString(), true);
        }

        protected virtual void WriteValue(IJsonSerializable obj)
        {
            string json = obj.ToJson();

            Debug.Assert(json != null);

            this.WriteCore(json, false);
        }

        protected virtual void WriteValue(ICollection items)
        {
            if ((items == null) || (items.Count == 0))
            {
                this.WriteCore("[]", false);
            }
            else
            {
                this.StartArrayScope();

                foreach (object o in items)
                {
                    this.WriteValue(o);
                }

                this.EndScope();
            }
        }

        protected virtual void WriteValue(IDictionary<string, object> record)
        {
            if ((record == null) || (record.Count == 0))
            {
                this.WriteCore("{}", false);
            }
            else
            {
                this.StartObjectScope();

                foreach (string key in record.Keys)
                {
                    if (String.IsNullOrEmpty(key))
                    {
                        throw new ArgumentException("Key of unsupported type contained in the dictionary.");
                    }

                    this.WriteName(key);
                    this.WriteValue(record[key]);
                }

                this.EndScope();
            }
        }

        protected void WriteCore(string text, bool quotes)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();

                if (currentScope.Type == ScopeType.Array)
                {
                    if (currentScope.ObjectCount != 0)
                    {
                        this.writer.WriteTrimmed(", ");
                        this.writer.WriteLine();
                    }

                    currentScope.ObjectCount++;
                }
            }

            if (quotes)
            {
                this.writer.Write('"');
            }

            this.writer.Write(text);

            if (quotes)
            {
                this.writer.Write('"');
            }
        }

        private string EscapeString(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            StringBuilder b = null;

            int index = 0;
            int count = 0;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if ((c > 0xFF) || this.quotedCharacters[c])
                {
                    if (b == null)
                    {
                        b = new StringBuilder(s.Length + 24);
                    }

                    if (count > 0)
                    {
                        b.Append(s, index, count);
                    }

                    index = i + 1;
                    count = 0;

                    if (c > 0xFF)
                    {
                        if ((c == 0x2028) || (c == 0x2029))
                        {
                            // These characters are control characters that need to be escaped but it is not worth
                            // increasing the quoted character map to be many thousand elements long.
                            b.Append(@"\u" + ((int)c).ToString("x", CultureInfo.InvariantCulture).PadLeft(4, '0'));
                        }
                        else
                        {
                            b.Append(c);
                        }
                    }
                    else
                    {
                        b.Append(JsonWriter.QuotedCharacterMap[c]);
                    }
                }
                else
                {
                    count++;
                }
            }

            if (b != null)
            {
                if (count > 0)
                {
                    b.Append(s, index, count);
                }

                return b.ToString();
            }

            return s;
        }

        private void EndScope()
        {
            if (this.scopes.Count == 0)
            {
                throw new InvalidOperationException("No active scope to end.");
            }

            this.writer.WriteLine();
            this.writer.Indent--;

            Scope scope = this.scopes.Pop();
            if (scope.Type == ScopeType.Array)
            {
                this.writer.Write("]");
            }
            else
            {
                this.writer.Write("}");
            }
        }

        private void StartArrayScope()
        {
            this.StartScope(ScopeType.Array);
        }

        private void StartObjectScope()
        {
            this.StartScope(ScopeType.Object);
        }

        private void StartScope(ScopeType type)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if ((currentScope.Type == ScopeType.Array) &&
                    (currentScope.ObjectCount != 0))
                {
                    this.writer.WriteTrimmed(", ");
                }

                currentScope.ObjectCount++;
            }

            Scope scope = new Scope(type);

            this.scopes.Push(scope);

            if (type == ScopeType.Array)
            {
                this.writer.Write("[");
            }
            else
            {
                this.writer.Write("{");
            }

            this.writer.Indent++;
            this.writer.WriteLine();
        }

        private sealed class Scope
        {
            public readonly ScopeType Type;

            public int ObjectCount;

            public Scope(ScopeType type)
            {
                this.Type = type;
            }
        }
    }
}