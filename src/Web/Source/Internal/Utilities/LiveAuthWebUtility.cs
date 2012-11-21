namespace Microsoft.Live
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class LiveAuthWebUtility
    {
        private static readonly DateTimeOffset ReferenceDate = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromSeconds(0));

        /// <summary>
        /// Decrypt an authentication token and parse into a JsonWebToken object.
        /// </summary>
        public static bool ReadUserIdFromAuthenticationToken(
            string authenticationToken, 
            string clientSecret, 
            out string userId, 
            out LiveAuthException error)
        {
            userId = null;
            JsonWebToken jwt;
            bool succeeded = false;
            if (DecodeAuthenticationToken(authenticationToken, clientSecret, out jwt, out error))
            {
                userId = jwt.Claims.UserId;

                if (jwt.IsExpired)
                {
                    error = new LiveAuthException(AuthErrorCodes.SessionExpired, ErrorText.SessionExpired);
                }
                else
                {
                    succeeded = true;
                }
            }

            return succeeded;
        }

        /// <summary>
        /// Decrypt an authentication token and parse into a JsonWebToken object.
        /// </summary>
        public static bool DecodeAuthenticationToken(
            string authenticationToken,
            string clientSecret,
            out JsonWebToken token,
            out LiveAuthException error)
        {
            Debug.Assert(!string.IsNullOrEmpty(authenticationToken));
            Debug.Assert(!string.IsNullOrEmpty(clientSecret));

            token = null;
            error = null;

            Dictionary<int, string> keys = new Dictionary<int, string>();
            keys.Add(0, clientSecret);

            try
            {
                token = new JsonWebToken(authenticationToken, keys);
                return true;
            }
            catch (Exception ex)
            {
                error = new LiveAuthException(AuthErrorCodes.ClientError, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the expires-in value of a given session in seconds.
        /// </summary>
        public static string GetExpiresInString(DateTimeOffset expires)
        {
            return ((int)(expires - DateTimeOffset.UtcNow).TotalSeconds).ToString();
        }

        /// <summary>
        /// Gets the expires value of a given session in seconds.
        /// </summary>
        public static string GetExpiresString(DateTimeOffset expires)
        {
            return ((int)(expires - ReferenceDate).TotalSeconds).ToString();
        }

        /// <summary>
        /// Parses expires string into a DateTimeOffset object.
        /// </summary>
        public static DateTimeOffset ParseExpiresValue(string expiresString)
        {
            int expiresValue = 0;
            if (int.TryParse(expiresString, out expiresValue))
            {
                return ReferenceDate.AddSeconds(expiresValue);
            }
            else
            {
                return DateTimeOffset.UtcNow;
            }
        }
    }
}
