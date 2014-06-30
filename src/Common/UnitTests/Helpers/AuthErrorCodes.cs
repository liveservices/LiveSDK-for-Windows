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

namespace Microsoft.Live.UnitTest
{
    using System;

    /// <summary>
    /// Defines the error codes used by the library.
    /// These error codes match the ones used on the server.
    /// </summary>
    internal static class AuthErrorCodes
    {
        public const string AccessDenied = "access_denied";
        public const string ServerError = "server_error";
        public const string UnknownUser = "unknown_user";
    }
}