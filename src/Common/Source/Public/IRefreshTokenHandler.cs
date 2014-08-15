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
