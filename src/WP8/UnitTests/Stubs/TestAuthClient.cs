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

using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Threading;

namespace Microsoft.Live.UnitTest
{
    public class TestAuthClient : IAuthClient
    {
        internal static readonly string ClientId = "0000000068034211";
        internal static readonly string[] Scopes = { "wl.signin", "wl.basic", "wl.contacts_create", "wl.skydrive_update", "wl.offline_access" };
        internal static readonly string FakeAccessToken = "EwAoAq1DBAAUlbRWyAJjK5w968Ru3Cyt%2f6GvwXwAAcyf0TMN5dgEkwdsowUJexmnpsFxiChUj%2fZ0B9X170rNu5CIp7X1RtjH%2fVs7f8uYKwZQvUNY8YLj5bed1LPIaEJuRUKUMQuTz8DyMP%2fUQJqaYRp5Hv3GeiP%2bSQjBX1g8oNs6JjWIdL%2fyQfp9AaRwPrn8stOpXIsboC6r7OeGmq4VBAkuw8RcgK9gZ9hhabXydBZkZMWwNWm35WQp0n8KICti%2bo%2bXh5w5eSTRtTjSqFjCO4hQvc2TnJPSC2SZhHlPt9SI%2b8JDNFx5grXSJmTcRovAcv78FMl4Bn%2fnr8VhSf5l57nbTDMjotd7X6F7si67Vurp%2bYobOOiMIOl1XZLlOw0DZgAACN%2fHShbm3Eu%2f%2bABDFUGBR4adKUdg1V74deM9hgf92OCqlkVeVNggE5GuyCUS4%2bUGoGiE8vAUQk56hYvvnOYBWsocCMiK38ZE0g%2fpvB7k0cGonS8H6xlLP3tsE1w75mJCHNDp0HG2ZJ2J41HmxD0ko5PwCMXKoOAR%2f7AXgVgpZhf4%2bF1QogVVx6Zh87dQkkJmRN%2fv%2bIZUEtB7hdL4PV2m8eOSaGan3BxbEU%2b8zjTNkRhxA43JhcJXgw8ZE9yO%2f5aN0lq8yCUAGlByl80WLqdvFgaMMMWIP9QW07Y0ndmd7MbxXaIOVJ6SrF589QlreaKjfTN11lCkRdEK8IecNxo7BoIdKwAA";
        internal static readonly string FakeOldAccessToken = "EwAoAq1DBAAUlbRWyAJjK5w968Ru3Cyt%2f6GvwXwAAcyf0TMN5dgEkwdsowUJexmnpsFxiChUj%2fZ0B9X170rNu5CIp7X1RtjH%2fVs7f8uYKwZQvUNY8YLj5bed1LPIaEJuRUKUMQuTz8DyMP%2fUQJqaYRp5Hv3GeiP%2bSQjBX1g8oNs6JjWIdL%2fyQfp9AaRwPrn8stOpXIsboC6r7OeGmq4VBAkuw8RcgK9gZ9hhabXydBZkZMWwNWm35WQp0n8KICti%2bo%2bXh5w5eSTRtTjSqFjCO4hQvc2TnJPSC2SZhHlPt9SI%2b8JDNFx5grXSJmTcRovAcv78FMl4Bn%2fnr8VhSf5l57nbTDMjotd7X6F7si67Vurp%2bYobOOiMIOl1XZLlOw0DZgAACN%2fHShbm3Eu%2f%2bABDFUGBR4adKUdg1V74deM9hgf92OCqlkVeVNggE5GuyCUS4%2bUGoGiE8vAUQk56hYvvnOYBWsocCMiK38ZE0g%2fpvB7k0cGonS8H6xlLP3tsE1w75mJCHNDp0HG2ZJ2J41HmxD0ko5PwCMXKoOAR%2f7AXgVgpZhf4%2bF1QogVVx6Zh87dQkkJmRN%2fv%2bIZUEtB7hdL4PV2m8eOSaGan3BxbEU%2b8zjTNkRhxA43JhcJXgw8ZE9yO%2f5aN0lq8yCUAGlByl80WLqdvFgaMMMWIP9QW07Y0ndmd7MbxXaIOVJ6SrF589QlreaKjfTN11lCkRdEK8IecNxo7BoIdKwAA";
        internal static readonly string FakeAuthenticationToken = "eyJhbGciOiJIUzI1NiIsICJ0eXAiOiJKV1QiLCAia2lkIjoiMCJ9.eyJ2ZXIiOjEsICJpc3MiOiJ1cm46d2luZG93czpsaXZlaWQiLCAiZXhwIjoxMzEyMzk4Mzg3LCAiYXVkIjoiIiwgInVpZCI6IjYzOTJkNjY2MWMxZTcwZGJjMWNkYTkzNDAzMGYzNzdlIiwgInVybjptaWNyb3NvZnQ6YXBwdXJpIjoiIn0.6eGQWz4KRlqN8J8qT2SyoeEkJb7pXOM8iu9EiGCKrUo";
        internal static readonly string FakeOldAuthenticationToken = "eyJhbGciOiJIUzI1NiIsICJ0eXAiOiJKV1QiLCAia2lkIjoiMCJ9.eyJ2ZXIiOjEsICJpc3MiOiJ1cm46d2luZG93czpsaXZlaWQiLCAiZXhwIjoxMzEyMzk4Mzg3LCAiYXVkIjoiIiwgInVpZCI6IjYzOTJkNjY2MWMxZTcwZGJjMWNkYTkzNDAzMGYzNzdlIiwgInVybjptaWNyb3NvZnQ6YXBwdXJpIjoiIn0.6eGQWz4KRlqN8J8qT2SyoeEkJb7pXOM8iu9EiGCKrUo";
        internal static readonly string FakeRefreshToken = "!gs!YAAAHgA%24100g7orI2CFb!RP7anoV5yNid2uXJ3dJelO1x6kF!rX5uqWn8bZlehcu!NUQTFYZW4Vw2sUYzG02ntkYF3jv2FQi3G!lp9VhE13CuFf6gsYJgqKqhxAb!Rq92AOWQKNYWuLUzv1wmgyUYTxeo8jEAnEVY8K0GTK0TkzGv8nZhvwRo%24";
        internal static readonly string FakeOldRefreshToken = "!gs!YAAAHgA%2419BHQKzTYCfskuAGvrXdmgP3ymnllRH4TTQY8vp2QSNG6vkIhh0*Xa6KIokuXyoYNmRs*!ec*ZaPb1x6IpBSX6N5pX3lGrmqXGoMmZC7C1SahWVx9YIiIhR6fp1Q!Z2jhCaFAqwAdEsXJ6okPNczcHWUs63wHAT75lQyAi460iTig%24";

        private readonly LiveAuthClient liveAuthClient;

        public TestAuthClient(LiveAuthClient liveAuthClient)
        {
            this.liveAuthClient = liveAuthClient;
        }

        public bool CanSignOut
        {
            get
            {
                return true;
            }
        }

        public void AuthenticateAsync(string clientId, string scopes, bool silent, Action<string, Exception> callback)
        {
            if (silent)
            {
                if (callback != null)
                {
                    ThreadPool.QueueUserWorkItem(
                        (object state) =>
                        {
                            callback(GenerateUserUnknownResponse(this.liveAuthClient.RedirectUrl), null);
                        });
                }

                return;
            }
            else
            {
                string consentUrl = this.liveAuthClient.BuildLoginUrl(scopes, false);

                if (callback != null)
                {
                    ThreadPool.QueueUserWorkItem(
                        (object state) =>
                        {
                            Uri consentUri = new Uri(consentUrl, UriKind.Absolute);
                            var parameters = Helpers.ParseQueryString(consentUri.Query);
                            string responseData = GenerateValidResponse(parameters);

                            callback(responseData, null);
                        });
                }

                return;
            }
        }

        public LiveConnectSession LoadSession(LiveAuthClient authClient)
        {
            var appData = IsolatedStorageSettings.ApplicationSettings;
            LiveConnectSession session = null;
            if (appData.Values.Count > 0)
            {
                session = new LiveConnectSession(authClient);

                if (appData.Contains(AuthConstants.RefreshToken))
                {
                    session.RefreshToken = appData[AuthConstants.RefreshToken] as string;
                }
            }

            return session;
        }

        public void SaveSession(LiveConnectSession session)
        {
            if (session != null)
            {
                var appData = IsolatedStorageSettings.ApplicationSettings;
                if (!string.IsNullOrEmpty(session.RefreshToken))
                {
                    if (appData.Contains(AuthConstants.RefreshToken))
                    {
                        appData.Remove(AuthConstants.RefreshToken);
                    }
                    appData.Add(AuthConstants.RefreshToken, session.RefreshToken);
                }

                appData.Save();
            }
        }

        public void CloseSession()
        {
            var appData = IsolatedStorageSettings.ApplicationSettings;
            if (appData.Values.Count > 0)
            {
                if (appData.Contains(AuthConstants.RefreshToken))
                {
                    appData.Remove(AuthConstants.RefreshToken);
                }

                appData.Save();
            }
        }

        internal static string GenerateValidResponse(IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder(HttpUtility.UrlDecode(parameters[AuthConstants.Callback]));
            sb = sb.Append("#").Append(AuthConstants.AccessToken).Append("=").Append(TestAuthClient.FakeAccessToken);
            sb = sb.Append("&").Append(AuthConstants.TokenType).Append("=").Append("bearer");
            sb = sb.Append("&").Append(AuthConstants.AuthenticationToken).Append("=").Append(TestAuthClient.FakeAuthenticationToken);
            sb = sb.Append("&").Append(AuthConstants.ExpiresIn).Append("=").Append("3600");
            sb = sb.Append("&").Append(AuthConstants.Scope).Append("=").Append(parameters[AuthConstants.Scope]);

            return sb.ToString();
        }

        private static string GenerateUserUnknownResponse(string redirectUrl)
        {
            StringBuilder sb = new StringBuilder(redirectUrl);
            sb = sb.Append("#").Append(AuthConstants.Error).Append("=").Append(AuthErrorCodes.UnknownUser);

            return sb.ToString();
        }
    }
}
