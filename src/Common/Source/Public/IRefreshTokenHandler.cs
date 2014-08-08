namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for classes that save and retrieve a refresh token from 
    /// persistent storage such as a database.
    /// </summary>
    public interface IRefreshTokenHandler
    {
        /// <summary>
        /// This method should be implemented to save a refresh token and 
        /// the matching user ID in persistent storage such as a database.
        /// </summary>
        /// <param name="tokenInfo">A RefreshTokenInfo instance to be stored.</param>
        /// <returns>An async Task instance.</returns>
        Task SaveRefreshTokenAsync(RefreshTokenInfo tokenInfo);

        /// <summary>
        /// This method should be implemented to retrieve a refresh token and user ID from persistent
        /// storage such as a database. The user ID and refresh token must belong to the same user. 
        /// Return null if no refresh token is found for the desired user.
        /// If the user ID returned does not match the user ID in the current session, the on going operation will fail. 
        /// </summary>
        /// <returns>An async Task instance.</returns>
        Task<RefreshTokenInfo> RetrieveRefreshTokenAsync();
    }
}
