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
