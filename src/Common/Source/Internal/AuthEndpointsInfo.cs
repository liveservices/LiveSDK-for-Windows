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

    internal static class AuthEndpointsInfo
    {
        /// <summary>
        /// Authorize endpoint path.
        /// </summary>
        public const string AuthorizePath = "/oauth20_authorize.srf";

        /// <summary>
        /// Token endpoint path.
        /// </summary>
        public const string TokenPath = "/oauth20_token.srf";

        /// <summary>
        /// Logout endpoint path.
        /// </summary>
        public const string LogoutPath = "/oauth20_logout.srf";

        /// <summary>
        /// Desktop redirect path.
        /// </summary>
        public const string DesktopRedirectPath = "/oauth20_desktop.srf";

        /// <summary>
        /// Auth host name.
        /// </summary>
        public const string AuthEndpoint = "https://login.live.com";
    }
}
