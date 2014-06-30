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
