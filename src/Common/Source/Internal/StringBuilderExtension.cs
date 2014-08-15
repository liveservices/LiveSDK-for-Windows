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

namespace Microsoft.Live
{
    using System.Diagnostics;
    using System.Text;

    internal static class StringBuilderExtension
    {
        private const char ForwardSlash = '/';

        /// <summary>
        /// Appends the path to the end of the StringBuilder.
        /// Removes trailing / on the given path.
        /// Makes sure there is a / between the end of the given sb and the given path.
        /// </summary>
        public static StringBuilder AppendUrlPath(this StringBuilder sb, string path)
        {
            if (path == null)
            {
                return sb;
            }

            if (sb.Length == 0)
            {
                return sb.Append(path.TrimEnd(ForwardSlash));
            }

            if (sb[sb.Length - 1] != ForwardSlash)
            {
                sb.Append(ForwardSlash);
            }

            return sb.Append(path.Trim(ForwardSlash));
        }

        /// <summary>
        /// Appends key=value to the string.
        /// Does not append ? or &amp; before key=value.
        /// </summary>
        public static StringBuilder AppendQueryParam(this StringBuilder sb, string key, string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            Debug.Assert(value != null);

            return sb.Append(key).Append('=').Append(value);
        }
    }
}
