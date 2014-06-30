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

namespace Microsoft.Live.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    internal class Helpers
    {
        public static IDictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(query))
            {
                query = query.TrimStart(new char[] { '?', '#' });
                if (!String.IsNullOrEmpty(query))
                {
                    string[] parameters = query.Split(new char[] { '&' });
                    foreach (string parameter in parameters)
                    {
                        string[] pair = parameter.Split(new char[] { '=' });
                        if (pair.Length == 2)
                        {
                            values.Add(pair[0], pair[1]);
                        }
                    }
                }
            }

            return values;
        }

    }
}
