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
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    internal static partial class LiveAuthUtility
    {
        private const string AppReqStateStringPrefix = AuthConstants.AppState + "=";
        private static readonly char[] ScopeSeparators = new char[] { ' ', ',' };

        /// <summary>
        /// Constructs an authorize url.
        /// </summary>
        public static string BuildWebAuthorizeUrl(
            string clientId,
            string redirectUrl,
            IEnumerable<string> scopes,
            DisplayType display,
            string locale,
            string state)
        {
            return BuildAuthorizeUrl(clientId, redirectUrl, scopes, ResponseType.Code, display, ThemeType.None, locale, state);
        }

        /// <summary>
        /// Constructs an authorize url.
        /// </summary>
        public static string BuildAuthorizeUrl(
            string clientId, 
            string redirectUrl, 
            IEnumerable<string> scopes, 
            ResponseType responseType,
            DisplayType display,
            ThemeType theme,
            string locale,
            string state)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));
            Debug.Assert(!string.IsNullOrEmpty(locale));

            IDictionary<string, string> options = new Dictionary<string, string>();
            options[AuthConstants.ClientId] = clientId;
            options[AuthConstants.Callback] = redirectUrl;
            options[AuthConstants.Scope] = BuildScopeString(scopes);
            options[AuthConstants.ResponseType] = responseType.ToString().ToLowerInvariant();
            options[AuthConstants.Display] = display.ToString().ToLowerInvariant();
            options[AuthConstants.Locale] = locale;
            options[AuthConstants.ClientState] = EncodeAppRequestState(state);

            if (theme != ThemeType.None)
            {
                options[AuthConstants.Theme] = theme.ToString().ToLowerInvariant();
            }

            return BuildAuthUrl(AuthEndpointsInfo.AuthorizePath, options);
        }

        /// <summary>
        /// Constructs an auth token URL.
        /// </summary>
        public static string BuildTokenUrl()
        {
            return BuildAuthUrl(AuthEndpointsInfo.TokenPath, null);
        }

        /// <summary>
        /// Constructs an auth logout URL.
        /// </summary>
        public static string BuildLogoutUrl()
        {
            return BuildAuthUrl(AuthEndpointsInfo.LogoutPath, null);
        }

        /// <summary>
        /// Constructs an auth logout URL.
        /// </summary>
        public static string BuildLogoutUrl(string clientId, string redirectUrl)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));

            Dictionary<string, string> options = new Dictionary<string, string>();
            options[AuthConstants.ClientId] = clientId;
            options[AuthConstants.Callback] = redirectUrl;

            return BuildAuthUrl(AuthEndpointsInfo.LogoutPath, options);
        }

        /// <summary>
        /// Constructs a desktop redirect URL.
        /// </summary>
        public static string BuildDesktopRedirectUrl()
        {
            return BuildAuthUrl(AuthEndpointsInfo.DesktopRedirectPath, null);
        }

        /// <summary>
        /// Constructs an auth URL.
        /// </summary>
        public static string BuildAuthUrl(string endpointPath, IDictionary<string, string> options)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(endpointPath));
            
            StringBuilder urlBuilder = new StringBuilder();
#if DEBUG
            urlBuilder.Append((LiveAuthClient.AuthEndpointOverride ?? AuthEndpointsInfo.AuthEndpoint).TrimEnd('/'));
#else
            urlBuilder.Append(AuthEndpointsInfo.AuthEndpoint);
#endif

            urlBuilder.Append(endpointPath);

            if (options != null)
            {
                urlBuilder.Append('?');
                urlBuilder.Append(UrlDataEncode(options));
            }

            return urlBuilder.ToString();
        }

        /// <summary>
        /// Builds refresh token request POST content.
        /// </summary>
        public static string BuildRefreshTokenPostContent(
            string clientId, string clientSecret, string redirectUrl, string refreshToken, IEnumerable<string> scopes)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));
            Debug.Assert(!string.IsNullOrEmpty(refreshToken));

            Dictionary<string, string> data = new Dictionary<string, string>();
            data[AuthConstants.ClientId] = clientId;
            data[AuthConstants.Callback] = redirectUrl;
            data[AuthConstants.RefreshToken] = refreshToken;
            data[AuthConstants.GrantType] = AuthConstants.RefreshToken;

            if (!string.IsNullOrEmpty(clientSecret))
            {
                data[AuthConstants.ClientSecret] = clientSecret;
            }

            if (scopes != null)
            {
                data[AuthConstants.Scope] = BuildScopeString(scopes);
            }

            string postContent = UrlDataEncode(data);

            return postContent;
        }

        /// <summary>
        /// Builds exchange code request POST content.
        /// </summary>
        public static string BuildCodeTokenExchangePostContent(
            string clientId, string clientSecret, string redirectUrl, string authorizationCode)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));
            Debug.Assert(!string.IsNullOrEmpty(authorizationCode));

            Dictionary<string, string> data = new Dictionary<string, string>();
            data[AuthConstants.ClientId] = clientId;
            data[AuthConstants.Callback] = redirectUrl;
            data[AuthConstants.Code] = authorizationCode;
            data[AuthConstants.GrantType] = AuthConstants.AuthorizationCode;

            if (!string.IsNullOrEmpty(clientSecret))
            {
                data[AuthConstants.ClientSecret] = clientSecret;
            }

            string postContent = UrlDataEncode(data);

            return postContent;
        }
        
        /// <summary>
        /// Converts a list of offers into one single offer string with space as separator.
        /// </summary>
        public static string BuildScopeString(IEnumerable<string> scopes)
        {
            StringBuilder sb = new StringBuilder();
            if (scopes != null)
            {
                bool firstScope = true;
                foreach (string s in scopes)
                {
                    if (firstScope)
                    {
                        firstScope = false;
                    }
                    else
                    {
                        sb.Append(ScopeSeparators[0]);
                    }

                    sb.Append(s);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a single offer string into a list of offers.
        /// </summary>
        public static IEnumerable<string> ParseScopeString(string scopesString)
        {
            if (!string.IsNullOrWhiteSpace(scopesString))
            {
                return new List<string>(
                    scopesString.Split(ScopeSeparators, StringSplitOptions.RemoveEmptyEntries));
            }

            return new List<string>();
        }

        /// <summary>
        /// Tries to extract the original redirect URL from current Url by removing some auth query parameters.
        /// </summary>
        public static string GetCurrentRedirectUrl(Uri uri)
        {
            string url = uri.AbsoluteUri;
            int idx = url.IndexOf('?');
            if (idx < 0)
            {
                return url;
            }

            string redirectPath = url.Substring(0, idx);
            string[] queryParts = url.Substring(idx + 1).Split('&');
            StringBuilder querySB = new StringBuilder();
            bool isFirstPart = true;
            for (int i = 0; i < queryParts.Length; i++)
            {
                string queryPart = queryParts[i];
                if (queryPart.StartsWith(AuthConstants.Code + '=') ||
                    queryPart.StartsWith(AuthConstants.ClientState + '='))
                {
                    continue;
                }

                if (isFirstPart)
                {
                    isFirstPart = false;
                }
                else
                {
                    querySB.Append('&');
                }

                querySB.Append(queryPart);
            }

            if (querySB.Length > 0)
            {
                redirectPath += '?' + querySB.ToString();
            }

            return redirectPath;
        }

        /// <summary>
        /// Parse an request states.
        /// </summary>
        public static IDictionary<string, string> DecodeAppRequestStates(string clientState)
        {
            var states = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(clientState))
            {
                string[] subStates = clientState.Split('&');
                foreach (string subState in subStates)
                {
                    string[] kv = subState.Split('=');
                    if (kv.Length == 2)
                    {
                        states.Add(kv[0], Uri.UnescapeDataString(kv[1]));
                    }
                }
            }

            return states;
        }

        /// <summary>
        /// Builds an auth state string for a given app state.
        /// </summary>
        public static string EncodeAppRequestState(string state)
        {
            if (state == null)
            {
                return string.Empty;
            }
            else
            {
                return AppReqStateStringPrefix + Uri.EscapeDataString(state);
            }
        }

        /// <summary>
        /// Checks if the first scopes set is a subset of the second scopes set.
        /// </summary>
        public static bool IsSubsetOfScopeRange(IEnumerable<string> scopes1, IEnumerable<string> scopes2)
        {
            Debug.Assert(scopes2 != null);
            bool isSubSet = true;

            if (scopes1 != null)
            {
                foreach (string scope in scopes1)
                {
                    if (!scopes2.Contains<string>(scope.ToLowerInvariant()))
                    {
                        isSubSet = false;
                        break;
                    }
                }
            }

            return isSubSet;
        }

        /// <summary>
        /// Encodes a dictionary into a Url encoded string.
        /// </summary>
        private static string UrlDataEncode(IDictionary<string, string> data)
        {
            StringBuilder dataBuilder = new StringBuilder();
            bool firstParam = true;
            foreach (KeyValuePair<string, string> option in data)
            {
                string format = "&{0}={1}";
                if (firstParam)
                {
                    format = "{0}={1}";
                    firstParam = false;
                }

                dataBuilder.AppendFormat(format, option.Key, Uri.EscapeDataString(option.Value));
            }

            return dataBuilder.ToString();
        }
    }
}
