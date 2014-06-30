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

    /// <summary>
    /// This class encapsulates refresh token information including token value and its corresponding user ID.
    /// </summary>
    public class RefreshTokenInfo
    {
#if WEB
        /// <summary>
        /// Initializes a new instance of the RefreshTokenInfo class.
        /// </summary>
        /// <param name="refreshToken">The refresh token value.</param>
        /// <param name="userId">The user ID associated with the token.</param>
        public RefreshTokenInfo(string refreshToken, string userId)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(refreshToken, "refreshToken");
            LiveUtility.ValidateNotNullOrWhiteSpaceString(userId, "userId");

            this.RefreshToken = refreshToken;
            this.UserId = userId;
        }
#elif DESKTOP
        /// <summary>
        /// Initializes a new instance of the RefreshTokenInfo class.
        /// </summary>
        /// <param name="refreshToken">The refresh token value.</param>
        public RefreshTokenInfo(string refreshToken)
        {
            LiveUtility.ValidateNotNullOrWhiteSpaceString(refreshToken, "refreshToken");

            this.RefreshToken = refreshToken;
        }
#endif

        /// <summary>
        /// Gets the User ID value.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the refresh token value.
        /// </summary>
        public string RefreshToken { get; private set; }
    }
}
