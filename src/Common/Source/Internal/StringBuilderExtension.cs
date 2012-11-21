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
