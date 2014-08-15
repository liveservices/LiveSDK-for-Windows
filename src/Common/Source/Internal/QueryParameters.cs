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
