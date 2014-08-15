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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    internal static class LiveUtility
    {
        /// <summary>
        /// Validates a given string parameter where null is not allowed.
        /// </summary>
        public static void ValidateNotNullParameter(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Validates a given Enumerable string parameter where null or empty is not allowed.
        /// </summary>
        public static void ValidateNotEmptyStringEnumeratorArguement(IEnumerable<string> value, string paramName)
        {
            ValidateNotNullParameter(value, paramName);
            if (!value.GetEnumerator().MoveNext())
            {
                throw new ArgumentException(ErrorText.ParameterInvalidEnumerablerEmpty);
            }
        }
        
        /// <summary>
        /// Validates a given string parameter where null or white space is not allowed.
        /// </summary>
        public static void ValidateNotNullOrWhiteSpaceString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (value == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                else
                {
                    throw new ArgumentException(ErrorText.ParameterInvalidStringNullOrEmpty, paramName);
                }
            }
        }

        /// <summary>
        /// Validate a given Url parameter value.
        /// </summary>
        public static void ValidateUrl(string value, string paramName)
        {
            ValidateNotNullOrWhiteSpaceString(value, paramName);
            if (!value.StartsWith(Uri.UriSchemeHttp, StringComparison.InvariantCultureIgnoreCase) &&
                !value.StartsWith(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(ErrorText.ParameterInvalidUrlFormat, paramName);
            }
        }
    }
}
