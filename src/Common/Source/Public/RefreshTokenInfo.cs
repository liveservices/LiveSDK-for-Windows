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
