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
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     Responsible for deserializing from JSON.
    /// </summary>
    internal sealed class JsonReader : IDisposable
    {
        /// <summary>
        ///     The max depth a JSON object originating from the user can be.
        /// </summary>
        private const int MaxUserJsonDepth = 10;

        /// <summary>
        ///     The reader containing the serialized JSON.
        /// </summary>
        private readonly TextReader reader;

        /// <summary>
        ///     The maximum depth the current object is allowed to be.
        ///     0 indicates no restriction.
        /// </summary>
        private readonly int maxObjectDepth;

        /// <summary>
        ///     The current depth of the object being parsed.
        /// </summary>
        private int currentDepth;

        /// <summary>
        ///     Whether this instance has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        ///     Initializes a new instance of the JsonReader class.
        /// </summary>
        /// <param name="text">The text to pull the JSON from.</param>
        public JsonReader(string text)
            : this(text, true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the JsonReader class.
        /// </summary>
        /// <param name="text">The text to pull the JSON from.</param>
        /// <param name="trusted">Whether the JSON was received trusted source.</param>
        public JsonReader(string text, bool trusted)
        {
            this.reader = new StringReader(text);
            this.maxObjectDepth = trusted ? 0 : JsonReader.MaxUserJsonDepth;
        }

        /// <summary>
        ///     Disposes of resources used by the instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                if (this.reader != null)
                {
                    this.reader.Dispose();
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        ///     Returns an object containing the data in the stream.
        /// </summary>
        /// <returns>An object type depending on the type of the data.</returns>
        public object ReadValue()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("Object was disposed.");
            }

            this.currentDepth++;

            if (this.maxObjectDepth != 0 && this.currentDepth > this.maxObjectDepth)
            {
                throw new FormatException("JSON object is too deep.");
            }

            object value = null;
            bool allowNull = false;

            char ch = this.PeekNextSignificantCharacter();
            if (ch == '[')
            {
                value = this.ReadArray();
            }
            else if (ch == '{')
            {
                value = this.ReadObject();
            }
            else if (ch == '\'' || ch == '"')
            {
                value = this.ReadString();
            }
            else if (Char.IsDigit(ch) || ch == '-' || ch == '.')
            {
                value = this.ReadNumber();
            }
            else if (ch == 't' || ch == 'f')
            {
                value = this.ReadBoolean();
            }
            else if (ch == 'n')
            {
                this.ReadNull();
                allowNull = true;
            }

            if (value == null && !allowNull)
            {
                throw new FormatException("Invalid JSON text.");
            }

            this.currentDepth--;
            return value;
        }

        private char GetNextSignificantCharacter()
        {
            char ch;
            do
            {
                ch = this.ReadCharFromReader();
            }
            while (ch != '\0' && Char.IsWhiteSpace(ch));

            return ch;
        }

        private string GetCharacters(int count)
        {
            string s = String.Empty;
            for (int i = 0; i < count; i++)
            {
                char ch = this.ReadCharFromReader();
                if (ch == '\0')
                {
                    return null;
                }

                s += ch;
            }

            return s;
        }

        private char PeekNextSignificantCharacter()
        {
            char ch = this.PeekCharFromReader();
            while (ch != '\0' && Char.IsWhiteSpace(ch))
            {
                this.ReadCharFromReader();
                ch = this.PeekCharFromReader();
            }

            return ch;
        }

        private IList ReadArray()
        {
            IList array = new List<object>();

            // Consume the '['
            this.ReadCharFromReader();

            while (true)
            {
                char ch = this.PeekNextSignificantCharacter();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated array literal.");
                }

                if (ch == ']')
                {
                    this.ReadCharFromReader();
                    return array;
                }

                if (array.Count != 0)
                {
                    if (ch != ',')
                    {
                        throw new FormatException("Invalid array literal.");
                    }
                    else
                    {
                        this.ReadCharFromReader();
                    }
                }

                object item = this.ReadValue();
                array.Add(item);
            }
        }

        private bool ReadBoolean()
        {
            string s = this.ReadName(/* allowQuotes */ false);

            if (s != null)
            {
                if (s.Equals("true", StringComparison.Ordinal))
                {
                    return true;
                }
                else if (s.Equals("false", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            throw new FormatException("Invalid boolean literal.");
        }

        private char PeekCharFromReader()
        {
            int val = this.reader.Peek();

            if (val == -1)
            {
                throw new FormatException("Unexpected end of string.");
            }

            return (char)val;
        }

        private char ReadCharFromReader()
        {
            int val = this.reader.Read();

            if (val == -1)
            {
                throw new FormatException("Unexpected end of string.");
            }

            return (char)val;
        }

        private string ReadName(bool allowQuotes)
        {
            char ch = this.PeekNextSignificantCharacter();

            if (ch == '"' || ch == '\'')
            {
                if (allowQuotes)
                {
                    return this.ReadString();
                }
            }
            else
            {
                var sb = new StringBuilder();

                while (true)
                {
                    ch = this.PeekCharFromReader();
                    if (ch == '_' || Char.IsLetterOrDigit(ch))
                    {
                        this.ReadCharFromReader();
                        sb.Append(ch);
                    }
                    else
                    {
                        return sb.ToString();
                    }
                }
            }

            return null;
        }

        private void ReadNull()
        {
            string s = this.ReadName(/* allowQuotes */ false);

            if ((s == null) || !s.Equals("null", StringComparison.Ordinal))
            {
                throw new FormatException("Invalid null literal.");
            }
        }

        private object ReadNumber()
        {
            char ch = this.ReadCharFromReader();
            bool isFloat = (ch == '.');

            var sb = new StringBuilder();

            sb.Append(ch);
            while (true)
            {
                ch = this.PeekNextSignificantCharacter();

                if (Char.IsDigit(ch))
                {
                    // Do nothing
                }
                else if (ch == '.' || ch == '+' || ch == '-' || char.ToLowerInvariant(ch) == 'e')
                {
                    // These characters are only allowed in floats (keep in mind the very first
                    // character will not come through here)
                    isFloat = true;
                }
                else
                {
                    break;
                }

                this.ReadCharFromReader();
                sb.Append(ch);
            }

            string s = sb.ToString();
            if (isFloat)
            {
                float value;
                if (Single.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }
            }
            else
            {
                int value;
                if (Int32.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                long val;
                if (Int64.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                {
                    return val;
                }
            }

            throw new FormatException("Invalid numeric literal.");
        }

        private IDictionary<string, object> ReadObject()
        {
            IDictionary<string, object> record = new DynamicDictionary();

            // Consume the '{'
            this.ReadCharFromReader();

            while (true)
            {
                char ch = this.PeekNextSignificantCharacter();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated object literal.");
                }

                if (ch == '}')
                {
                    this.ReadCharFromReader();
                    return record;
                }

                if (record.Count != 0)
                {
                    if (ch != ',')
                    {
                        throw new FormatException("Invalid object literal.");
                    }
                    else
                    {
                        this.ReadCharFromReader();
                    }
                }

                string name = this.ReadName(/* allowQuotes */ true);
                ch = this.PeekNextSignificantCharacter();

                if (ch != ':')
                {
                    throw new FormatException("Unexpected name/value pair syntax in object literal.");
                }
                else
                {
                    this.ReadCharFromReader();
                }

                object item = this.ReadValue();
                record[name] = item;
            }
        }

        private string ReadString()
        {
            var sb = new StringBuilder();

            char endQuoteCharacter = this.ReadCharFromReader();
            bool inEscape = false;

            while (true)
            {
                char ch = this.ReadCharFromReader();
                if (ch == '\0')
                {
                    throw new FormatException("Unterminated string literal.");
                }

                if (inEscape)
                {
                    switch (ch)
                    {
                        case 'u':
                            string unicodeSequence = this.GetCharacters(4);
                            if (unicodeSequence == null)
                            {
                                throw new FormatException("Unterminated string literal.");
                            }

                            ch = (char)Int32.Parse(unicodeSequence, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                            break;
                        case 'b':
                            ch = '\b';
                            break;
                        case 't':
                            ch = '\t';
                            break;
                        case 'n':
                            ch = '\n';
                            break;
                        case 'f':
                            ch = '\f';
                            break;
                        case 'r':
                            ch = '\r';
                            break;
                    }

                    sb.Append(ch);
                    inEscape = false;
                    continue;
                }

                if (ch == '\\')
                {
                    inEscape = true;
                    continue;
                }

                if (ch == endQuoteCharacter)
                {
                    return sb.ToString();
                }

                sb.Append(ch);
            }
        }
    }
}