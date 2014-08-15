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
    using System.Threading.Tasks;

    /// <summary>
    /// An interface that defines the contract betwewn the LiveAuthClient class and the platform
    /// specific implementation of the authentication flow.
    /// </summary>
    internal interface IAuthClient
    {
        /// <summary>
        /// Gets a boolean indicating whether or not the user can be signed out.
        /// </summary>
        bool CanSignOut { get; }

#if NETFX_CORE
        /// <summary>
        /// Performs authentication flow using platform specific technology.
        /// </summary>
        Task<LiveLoginResult> AuthenticateAsync(string scopes, bool silent);
#else
        /// <summary>
        /// Performs authentication flow using platform specific technology.
        /// </summary>
        void AuthenticateAsync(string clientId, string scopes, bool silent, Action<string, Exception> callback);
#endif

        /// <summary>
        /// Saves the session data to app local storage.
        /// </summary>
        LiveConnectSession LoadSession(LiveAuthClient authClient);

        /// <summary>
        /// Loads the session data from app local storage.
        /// </summary>
        void SaveSession(LiveConnectSession session);

        /// <summary>
        /// Clears the session data in app local storage.
        /// </summary>
        void CloseSession();
    }
}
