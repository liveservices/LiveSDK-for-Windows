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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Live.Serialization;

    /// <summary>
    /// A utility class handles operations required to connect to auth server.
    /// </summary>
    internal static class LiveAuthRequestUtility
    {
        private const string TokenRequestContentType = "application/x-www-form-urlencoded;charset=UTF-8";

        /// <summary>
        /// An async method to exhange authorization code for auth tokens with the auth server.
        /// </summary>
        public static Task<LiveLoginResult> ExchangeCodeForTokenAsync(string clientId, string clientSecret, string redirectUrl, string authorizationCode)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));
            Debug.Assert(!string.IsNullOrEmpty(authorizationCode));

            string postContent = LiveAuthUtility.BuildCodeTokenExchangePostContent(clientId, clientSecret, redirectUrl, authorizationCode);
            return RequestAccessTokenAsync(postContent);
        }

        /// <summary>
        /// An async method to get auth tokens using refresh token.
        /// </summary>
        public static Task<LiveLoginResult> RefreshTokenAsync(
            string clientId, string clientSecret, string redirectUrl, string refreshToken,
            IEnumerable<string> scopes)
        {
            Debug.Assert(!string.IsNullOrEmpty(clientId));
            Debug.Assert(!string.IsNullOrEmpty(redirectUrl));
            Debug.Assert(!string.IsNullOrEmpty(refreshToken));

            string postContent = LiveAuthUtility.BuildRefreshTokenPostContent(clientId, clientSecret, redirectUrl, refreshToken, scopes);
            return RequestAccessTokenAsync(postContent);
        }

        private static Task<LiveLoginResult> RequestAccessTokenAsync(string postContent)
        {
            Task<LiveLoginResult> task = Task.Factory.StartNew(() =>
            {
                return RequestAccessToken(postContent);
            });

            return task;
        }

        private static LiveLoginResult RequestAccessToken(string postContent)
        {
            string url = LiveAuthUtility.BuildTokenUrl();
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = null;
            LiveLoginResult loginResult = null;
            request.Method = ApiMethod.Post.ToString().ToUpperInvariant();
            request.ContentType = TokenRequestContentType;

            try
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(postContent);
                }

                response = request.GetResponse() as HttpWebResponse;
                loginResult = ReadResponse(response);
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
                if (response != null)
                {
                    loginResult = ReadResponse(response);
                }
            }
            catch (IOException ioe)
            {
                loginResult = new LiveLoginResult(new LiveAuthException(AuthErrorCodes.ClientError, ioe.Message));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            if (loginResult == null)
            {
                loginResult = new LiveLoginResult(new LiveAuthException(AuthErrorCodes.ClientError, ErrorText.RetrieveTokenError));
            }

            return loginResult;
        }

        private static LiveLoginResult ReadResponse(HttpWebResponse response)
        {
            LiveConnectSession newSession = null;
            LiveConnectSessionStatus status = LiveConnectSessionStatus.Unknown;
            IDictionary<string, object> jsonObj = null;
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    JsonReader jsReader = new JsonReader(reader.ReadToEnd());
                    jsonObj = jsReader.ReadValue() as IDictionary<string, object>;
                }
            }
            catch (FormatException fe)
            {
                return new LiveLoginResult(
                    new LiveAuthException(AuthErrorCodes.ServerError, fe.Message));
            }

            if (jsonObj != null)
            {
                if (jsonObj.ContainsKey(AuthConstants.Error))
                {
                    string errorCode = jsonObj[AuthConstants.Error] as string;
                    string errorDescription = string.Empty;
                    if (jsonObj.ContainsKey(AuthConstants.ErrorDescription))
                    {
                        errorDescription = jsonObj[AuthConstants.ErrorDescription] as string;
                    }

                    return new LiveLoginResult(new LiveAuthException(errorCode, errorDescription));
                }
                else
                {
                    status = LiveConnectSessionStatus.Connected;
                    newSession = CreateSession(jsonObj);
                    return new LiveLoginResult(status, newSession);
                }
            }

            return new LiveLoginResult(
                    new LiveAuthException(AuthErrorCodes.ServerError, ErrorText.ServerError));
        }

        /// <summary>
        /// Creates a LiveConnectSession object based on the parsed response.
        /// </summary>
        private static LiveConnectSession CreateSession(IDictionary<string, object> result)
        {
            var session = new LiveConnectSession();

            Debug.Assert(result.ContainsKey(AuthConstants.AccessToken));
            if (result.ContainsKey(AuthConstants.AccessToken))
            {
                session.AccessToken = result[AuthConstants.AccessToken] as string;

                if (result.ContainsKey(AuthConstants.AuthenticationToken))
                {
                    session.AuthenticationToken = result[AuthConstants.AuthenticationToken] as string;
                }

                if (result.ContainsKey(AuthConstants.ExpiresIn))
                {
                    if (result[AuthConstants.ExpiresIn] is string)
                    {
                        session.Expires = CalculateExpiration(result[AuthConstants.ExpiresIn] as string);
                    }
                    else
                    {
                        session.Expires = DateTimeOffset.UtcNow.AddSeconds((int)result[AuthConstants.ExpiresIn]);
                    }
                }

                if (result.ContainsKey(AuthConstants.Scope))
                {
                    session.Scopes =
                        LiveAuthUtility.ParseScopeString(result[AuthConstants.Scope] as string);
                }

                if (result.ContainsKey(AuthConstants.RefreshToken))
                {
                    session.RefreshToken = result[AuthConstants.RefreshToken] as string;
                }
            }

            return session;
        }

        /// <summary>
        /// Calculates when the access token will be expired.
        /// </summary>
        private static DateTimeOffset CalculateExpiration(string expiresIn)
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow;
            long seconds;
            if (long.TryParse(expiresIn, out seconds))
            {
                expires = expires.AddSeconds(seconds);
            }

            return expires;
        }
    }
}
