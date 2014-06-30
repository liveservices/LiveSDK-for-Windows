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
