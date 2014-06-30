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
    using System.Collections.Generic;

    internal static class QueryParameters
    {
        #region Fields

        /// <summary>
        /// The key name of the method query parameter.
        /// </summary>
        public const string Method = "method";

        /// <summary>
        /// The key name of the overwrite query parameter.
        /// </summary>
        public const string Overwrite = "overwrite";

        /// <summary>
        /// The key name of the suppress response codes query parameter.
        /// </summary>
        public const string SuppressResponseCodes = "suppress_response_codes";

        /// <summary>
        /// The key name of the suppress redirects query parameter.
        /// </summary>
        public const string SuppressRedirects = "suppress_redirects";

        private static readonly Dictionary<OverwriteOption, string> uploadOptionToOverwriteValue;

        #endregion

        #region Constructors

        static QueryParameters()
        {
            uploadOptionToOverwriteValue = new Dictionary<OverwriteOption, string>();
            uploadOptionToOverwriteValue[OverwriteOption.Rename] = "choosenewname";
            uploadOptionToOverwriteValue[OverwriteOption.Overwrite] = "true";
            uploadOptionToOverwriteValue[OverwriteOption.DoNotOverwrite] = "false";
        }

        #endregion

        #region Methods

        public static string GetOverwriteValue(OverwriteOption option)
        {
            return uploadOptionToOverwriteValue[option];
        }

        #endregion
    }
}
