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
