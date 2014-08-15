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
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web;

    /// <summary>
    /// A utility class that handles read and write operations on current web session.
    /// </summary>
    internal static class HttpContextUtility
    {
        private const string AuthCookie = "wl_auth";

        /// <summary>
        /// Reads authorization code from current session.
        /// </summary>
        public static bool ReadAuthCodeRequest(
            HttpContextBase webContext,
            out string code,
            out string state,
            out string requestTs,
            out LiveAuthException error)
        {
            Debug.Assert(webContext != null);

            NameValueCollection queryString = webContext.Request.QueryString;
            
            code = queryString[AuthConstants.Code];
            bool isCodeRequest = !string.IsNullOrEmpty(code);
            
            string clientState = queryString[AuthConstants.ClientState];
            IDictionary<string, string> states = LiveAuthUtility.DecodeAppRequestStates(clientState);
            state = states.ContainsKey(AuthConstants.AppState) ? states[AuthConstants.AppState] : null;
            requestTs = states.ContainsKey(AuthConstants.ClientRequestTs) ? states[AuthConstants.ClientRequestTs] : null;
            
            string errorCode = queryString[AuthConstants.Error];
            string errorDescription = queryString[AuthConstants.ErrorDescription];
            error = string.IsNullOrEmpty(errorCode) ? null : new LiveAuthException(errorCode, errorDescription, state);

            return isCodeRequest;
        }

        /// <summary>
        /// Check if current session has a token request.
        /// </summary>
        public static bool ReadRefreshTokenRequest(
            HttpContextBase webContext, 
            out string clientId,
            out IEnumerable<string> scopes)
        {
            clientId = null;
            scopes = null;
            bool isTokenRequest = false;
            if (webContext != null)
            {
                NameValueCollection queryString = webContext.Request.QueryString;
                string requestToken = queryString[AuthConstants.ResponseType];
                isTokenRequest = (requestToken == AuthConstants.Token);
                if (isTokenRequest)
                {
                    clientId = queryString[AuthConstants.ClientId];
                    // If this is sent by the client library, the token response should honor the scope parameter.
                    scopes = LiveAuthUtility.ParseScopeString(queryString[AuthConstants.Scope]);
                }
            }

            return isTokenRequest;
        }

        /// <summary>
        /// Reads current user session.
        /// </summary>
        public static LiveLoginResult GetUserLoginStatus(HttpContextBase webContext)
        {
            Debug.Assert(webContext != null);

            HttpCookie cookie = webContext.Request.Cookies[AuthCookie];
            LiveConnectSession session = null;
            LiveConnectSessionStatus status = LiveConnectSessionStatus.Unknown;
            if (cookie != null && cookie.Values != null)
            {
                string accessToken = cookie[AuthConstants.AccessToken];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    session = new LiveConnectSession();
                    session.AccessToken = UrlDataDecode(accessToken);
                    session.AuthenticationToken = UrlDataDecode(cookie[AuthConstants.AuthenticationToken]);
                    session.RefreshToken = UrlDataDecode(cookie[AuthConstants.RefreshToken]);
                    session.Scopes = LiveAuthUtility.ParseScopeString(UrlDataDecode(cookie[AuthConstants.Scope]));
                    session.Expires = LiveAuthWebUtility.ParseExpiresValue(UrlDataDecode(cookie[AuthConstants.Expires]));
                    status = session.IsValid ? LiveConnectSessionStatus.Connected : LiveConnectSessionStatus.Expired;
                }
                else
                {
                    // If we previously recorded NotConnected, take that value.
                    // Ignore other values that may be set by JS library.                            
                    LiveConnectSessionStatus statusFromCookie;
                    if (Enum.TryParse<LiveConnectSessionStatus>(cookie[AuthConstants.Status],
                            true/*ignore case*/,
                            out statusFromCookie))
                    {
                        if (statusFromCookie == LiveConnectSessionStatus.NotConnected)
                        {
                            status = statusFromCookie;
                        }
                    }
                }
            }

            return new LiveLoginResult(status, session);            
        }

        /// <summary>
        /// Writes the user current session.
        /// </summary>
        public static void UpdateUserSession(HttpContextBase context, LiveLoginResult loginResult, string requestTs)
        {
            if (context == null)
            {
                return;
            }

            Debug.Assert(loginResult != null);

            Dictionary<string, string> cookieValues = new Dictionary<string, string>();
            HttpCookie cookie = context.Request.Cookies[AuthCookie];
            HttpCookie newCookie = new HttpCookie(AuthCookie);
            newCookie.Path = "/";
            string host = context.Request.Headers["Host"];
            newCookie.Domain = host.Split(':')[0];

            if (cookie != null && cookie.Values != null)
            {
                foreach (string key in cookie.Values.AllKeys)
                {
                    newCookie.Values[key] = cookie[key];
                }
            }

            LiveConnectSession session = loginResult.Session;
            if (session != null)
            {
                newCookie.Values[AuthConstants.AccessToken] = Uri.EscapeDataString(session.AccessToken);
                newCookie.Values[AuthConstants.AuthenticationToken] = Uri.EscapeDataString(session.AuthenticationToken);
                newCookie.Values[AuthConstants.Scope] = Uri.EscapeDataString(LiveAuthUtility.BuildScopeString(session.Scopes));
                newCookie.Values[AuthConstants.ExpiresIn] = Uri.EscapeDataString(LiveAuthWebUtility.GetExpiresInString(session.Expires));
                newCookie.Values[AuthConstants.Expires] = Uri.EscapeDataString(LiveAuthWebUtility.GetExpiresString(session.Expires));
            }

            LiveConnectSessionStatus status;
            if (!string.IsNullOrEmpty(newCookie[AuthConstants.AccessToken]))
            {
                // We have an access token, so it is connected, regardless expired or not 
                // since it is handled after loading the session in both Asp.Net and JS library.
                status = LiveConnectSessionStatus.Connected;
            }
            else
            {
                status = loginResult.Status;
                if (loginResult.Status == LiveConnectSessionStatus.Unknown)
                {
                    // If we recorded NotConnected previously, keep it.
                    LiveConnectSessionStatus statusFromCookie;
                    if (Enum.TryParse<LiveConnectSessionStatus>(
                            newCookie[AuthConstants.Status], 
                            true/*ignore case*/, 
                            out statusFromCookie))
                    {
                        if (statusFromCookie == LiveConnectSessionStatus.NotConnected)
                        {
                            status = statusFromCookie;
                        }
                    }
                }
            }

            newCookie.Values[AuthConstants.Status] = GetStatusString(status);

            // Needs to write error to inform the JS library.
            LiveAuthException authError = loginResult.Error as LiveAuthException;
            if (authError != null)
            {
                newCookie.Values[AuthConstants.Error] = Uri.EscapeDataString(authError.ErrorCode);
                newCookie.Values[AuthConstants.ErrorDescription] = HttpUtility.UrlPathEncode(authError.Message);
            }
            else if (status != LiveConnectSessionStatus.Connected)
            {
                newCookie.Values[AuthConstants.Error] = Uri.EscapeDataString(AuthErrorCodes.AccessDenied);
                newCookie.Values[AuthConstants.ErrorDescription] = HttpUtility.UrlPathEncode("Cannot retrieve access token.");
            }

            if (!string.IsNullOrEmpty(requestTs))
            {
                newCookie.Values[AuthConstants.ClientRequestTs] = requestTs;
            }

            context.Response.Cookies.Add(newCookie);
        }

        /// <summary>
        /// Clear current user session from the auth cookie.
        /// </summary>
        public static void ClearUserSession(HttpContextBase context)
        {
            Debug.Assert(context != null);

            if (context.Request.Cookies[AuthCookie] != null)
            {
                HttpCookie authCookie = new HttpCookie(AuthCookie);
                authCookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies.Add(authCookie);
            }
        }

        private static string UrlDataDecode(string value)
        {
            if (value == null)
            {
                return null;
            }

            return Uri.UnescapeDataString(value);
        }

        private static string GetStatusString(LiveConnectSessionStatus status)
        {
            // We need to write cookie status values that conform to the JS library format.
            switch (status)
            {
                case LiveConnectSessionStatus.NotConnected:                    
                    return "notConnected";
                default:
                    return status.ToString().ToLowerInvariant();
            }
        }
    }
}
