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
            object clientSecrets, 
            out string userId, 
            out LiveAuthException error)
        {
            userId = null;
            JsonWebToken jwt;
            bool succeeded = false;
            if (DecodeAuthenticationToken(authenticationToken, clientSecrets, out jwt, out error))
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
            object clientSecrets,
            out JsonWebToken token,
            out LiveAuthException error)
        {
            Debug.Assert(!string.IsNullOrEmpty(authenticationToken));
            Debug.Assert(clientSecrets != null);

            token = null;
            error = null;

            try
            {
                token = new JsonWebToken(authenticationToken, clientSecrets);
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
