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
