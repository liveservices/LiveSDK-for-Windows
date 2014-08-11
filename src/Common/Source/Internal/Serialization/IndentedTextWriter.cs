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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     Represents a text writer that supports indentation.
    /// </summary>
    internal sealed class IndentedTextWriter : TextWriter
    {
        /// <summary>
        ///     The underlying text writer.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        ///     Indicates whether the text should be minimized.
        /// </summary>
        private readonly bool minimized;

        /// <summary>
        ///     The current indentation level.
        /// </summary>
        private int indentLevel;

        /// <summary>
        ///     Indicates whether tabs are pending.
        /// </summary>
        private bool tabsPending;

        /// <summary>
        ///     The string used for tabs.
        /// </summary>
        private string tabString;

        /// <summary>
        ///     Initializes a new instance of the IndentedTextWriter class.
        /// </summary>
        public IndentedTextWriter(TextWriter writer, bool minimized)
            : base(CultureInfo.InvariantCulture)
        {
            this.writer = writer;
            this.minimized = minimized;
            this.NewLine = minimized ? "" : "\r";
            this.tabString = "   ";
            this.indentLevel = 0;
            this.tabsPending = false;
        }

        public override Encoding Encoding
        {
            get
            {
                return this.writer.Encoding;
            }
        }

        public override string NewLine
        {
            get
            {
                return this.writer.NewLine;
            }

            set
            {
                this.writer.NewLine = value;
            }
        }

        public int Indent
        {
            get
            {
                return this.indentLevel;
            }

            set
            {
                Debug.Assert(value >= 0);

                if (value < 0)
                {
                    value = 0;
                }

                this.indentLevel = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.writer.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Flush()
        {
            this.writer.Flush();
        }

        public override void Write(string s)
        {
            this.OutputTabs();
            this.writer.Write(s);
        }

        public override void Write(bool value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(char value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            this.OutputTabs();
            this.writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.OutputTabs();
            this.writer.Write(buffer, index, count);
        }

        public override void Write(double value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(float value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(int value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(long value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(object value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public override void Write(string format, params object[] arg)
        {
            this.OutputTabs();
            this.writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        public void WriteLineNoTabs(string s)
        {
            this.writer.WriteLine(s);
        }

        public override void WriteLine(string s)
        {
            this.OutputTabs();
            this.writer.WriteLine(s);
            this.tabsPending = true;
        }

        public override void WriteLine()
        {
            this.OutputTabs();
            this.writer.WriteLine();
            this.tabsPending = true;
        }

        public override void WriteLine(bool value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(char value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(char[] buffer)
        {
            this.OutputTabs();
            this.writer.WriteLine(buffer);
            this.tabsPending = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.OutputTabs();
            this.writer.WriteLine(buffer, index, count);
            this.tabsPending = true;
        }

        public override void WriteLine(double value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(float value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(int value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(long value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(object value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            this.OutputTabs();
            this.writer.WriteLine(format, arg);
            this.tabsPending = true;
        }

        public override void WriteLine(UInt32 value)
        {
            this.OutputTabs();
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public void WriteTrimmed(string text)
        {
            if (this.minimized == false)
            {
                this.Write(text);
            }
            else
            {
                this.Write(text.Trim());
            }
        }

        private void OutputTabs()
        {
            if (this.tabsPending)
            {
                if (this.minimized == false)
                {
                    for (int i = 0; i < this.indentLevel; i++)
                    {
                        this.writer.Write(this.tabString);
                    }
                }

                this.tabsPending = false;
            }
        }
    }
}