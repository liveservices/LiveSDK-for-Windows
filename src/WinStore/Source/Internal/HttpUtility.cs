namespace Microsoft.Live
{
    using System;

    internal class HttpUtility
    {
        public static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str);
        }

        public static string UrlDecode(string str)
        {
            return Uri.UnescapeDataString(str);
        }
    }
}
