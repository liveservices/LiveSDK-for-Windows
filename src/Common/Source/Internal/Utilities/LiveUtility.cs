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
