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

namespace Microsoft.Live.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Storage;
    using Windows.System.Threading;

    public class TestAuthClient : IAuthClient
    {
        internal static readonly string TokenEndpoint = "https://oauth.live.com/token";
        internal static readonly string FakeAccessToken = "EwAoAq1DBAAUlbRWyAJjK5w968Ru3Cyt%2f6GvwXwAAcyf0TMN5dgEkwdsowUJexmnpsFxiChUj%2fZ0B9X170rNu5CIp7X1RtjH%2fVs7f8uYKwZQvUNY8YLj5bed1LPIaEJuRUKUMQuTz8DyMP%2fUQJqaYRp5Hv3GeiP%2bSQjBX1g8oNs6JjWIdL%2fyQfp9AaRwPrn8stOpXIsboC6r7OeGmq4VBAkuw8RcgK9gZ9hhabXydBZkZMWwNWm35WQp0n8KICti%2bo%2bXh5w5eSTRtTjSqFjCO4hQvc2TnJPSC2SZhHlPt9SI%2b8JDNFx5grXSJmTcRovAcv78FMl4Bn%2fnr8VhSf5l57nbTDMjotd7X6F7si67Vurp%2bYobOOiMIOl1XZLlOw0DZgAACN%2fHShbm3Eu%2f%2bABDFUGBR4adKUdg1V74deM9hgf92OCqlkVeVNggE5GuyCUS4%2bUGoGiE8vAUQk56hYvvnOYBWsocCMiK38ZE0g%2fpvB7k0cGonS8H6xlLP3tsE1w75mJCHNDp0HG2ZJ2J41HmxD0ko5PwCMXKoOAR%2f7AXgVgpZhf4%2bF1QogVVx6Zh87dQkkJmRN%2fv%2bIZUEtB7hdL4PV2m8eOSaGan3BxbEU%2b8zjTNkRhxA43JhcJXgw8ZE9yO%2f5aN0lq8yCUAGlByl80WLqdvFgaMMMWIP9QW07Y0ndmd7MbxXaIOVJ6SrF589QlreaKjfTN11lCkRdEK8IecNxo7BoIdKwAA";
        internal static readonly string FakeOldAccessToken = "EwAoAq1DBAAUlbRWyAJjK5w968Ru3Cyt%2f6GvwXwAAcyf0TMN5dgEkwdsowUJexmnpsFxiChUj%2fZ0B9X170rNu5CIp7X1RtjH%2fVs7f8uYKwZQvUNY8YLj5bed1LPIaEJuRUKUMQuTz8DyMP%2fUQJqaYRp5Hv3GeiP%2bSQjBX1g8oNs6JjWIdL%2fyQfp9AaRwPrn8stOpXIsboC6r7OeGmq4VBAkuw8RcgK9gZ9hhabXydBZkZMWwNWm35WQp0n8KICti%2bo%2bXh5w5eSTRtTjSqFjCO4hQvc2TnJPSC2SZhHlPt9SI%2b8JDNFx5grXSJmTcRovAcv78FMl4Bn%2fnr8VhSf5l57nbTDMjotd7X6F7si67Vurp%2bYobOOiMIOl1XZLlOw0DZgAACN%2fHShbm3Eu%2f%2bABDFUGBR4adKUdg1V74deM9hgf92OCqlkVeVNggE5GuyCUS4%2bUGoGiE8vAUQk56hYvvnOYBWsocCMiK38ZE0g%2fpvB7k0cGonS8H6xlLP3tsE1w75mJCHNDp0HG2ZJ2J41HmxD0ko5PwCMXKoOAR%2f7AXgVgpZhf4%2bF1QogVVx6Zh87dQkkJmRN%2fv%2bIZUEtB7hdL4PV2m8eOSaGan3BxbEU%2b8zjTNkRhxA43JhcJXgw8ZE9yO%2f5aN0lq8yCUAGlByl80WLqdvFgaMMMWIP9QW07Y0ndmd7MbxXaIOVJ6SrF589QlreaKjfTN11lCkRdEK8IecNxo7BoIdKwAA";
        internal static readonly string FakeAuthenticationToken = "eyJhbGciOiJIUzI1NiIsICJ0eXAiOiJKV1QiLCAia2lkIjoiMCJ9.eyJ2ZXIiOjEsICJpc3MiOiJ1cm46d2luZG93czpsaXZlaWQiLCAiZXhwIjoxMzEyMzk4Mzg3LCAiYXVkIjoiIiwgInVpZCI6IjYzOTJkNjY2MWMxZTcwZGJjMWNkYTkzNDAzMGYzNzdlIiwgInVybjptaWNyb3NvZnQ6YXBwdXJpIjoiIn0.6eGQWz4KRlqN8J8qT2SyoeEkJb7pXOM8iu9EiGCKrUo";
        internal static readonly string FakeOldAuthenticationToken = "eyJhbGciOiJIUzI1NiIsICJ0eXAiOiJKV1QiLCAia2lkIjoiMCJ9.eyJ2ZXIiOjEsICJpc3MiOiJ1cm46d2luZG93czpsaXZlaWQiLCAiZXhwIjoxMzEyMzk4Mzg3LCAiYXVkIjoiIiwgInVpZCI6IjYzOTJkNjY2MWMxZTcwZGJjMWNkYTkzNDAzMGYzNzdlIiwgInVybjptaWNyb3NvZnQ6YXBwdXJpIjoiIn0.6eGQWz4KRlqN8J8qT2SyoeEkJb7pXOM8iu9EiGCKrUo";
        internal static readonly string FakeRefreshToken = "!gs!YAAAHgA%24100g7orI2CFb!RP7anoV5yNid2uXJ3dJelO1x6kF!rX5uqWn8bZlehcu!NUQTFYZW4Vw2sUYzG02ntkYF3jv2FQi3G!lp9VhE13CuFf6gsYJgqKqhxAb!Rq92AOWQKNYWuLUzv1wmgyUYTxeo8jEAnEVY8K0GTK0TkzGv8nZhvwRo%24";
        internal static readonly string FakeOldRefreshToken = "!gs!YAAAHgA%2419BHQKzTYCfskuAGvrXdmgP3ymnllRH4TTQY8vp2QSNG6vkIhh0*Xa6KIokuXyoYNmRs*!ec*ZaPb1x6IpBSX6N5pX3lGrmqXGoMmZC7C1SahWVx9YIiIhR6fp1Q!Z2jhCaFAqwAdEsXJ6okPNczcHWUs63wHAT75lQyAi460iTig%24";
        private string redirectUrl;

        public bool CanSignOut { get; private set; }

        public static IEnumerable<string> Scopes { get; set; }

        public async Task<LiveLoginResult> AuthenticateAsync(string redirectUrl, bool silent)
        {
            this.redirectUrl = redirectUrl;

            LiveLoginResult result;
            if (silent)
            {
                result = new LiveLoginResult(LiveConnectSessionStatus.Unknown, null);
            }
            else
            {
                LiveConnectSession session = new LiveConnectSession();
                session.AccessToken = TestAuthClient.FakeAccessToken;
                session.AuthenticationToken = TestAuthClient.FakeAuthenticationToken;
                result = new LiveLoginResult(LiveConnectSessionStatus.Connected, session);
            }

            return await Task.Factory.StartNew<LiveLoginResult>(() =>
            {
                return result;
            });
        }

        public void CloseSession()
        {
            ApplicationDataContainer appData = ApplicationData.Current.RoamingSettings;
            if (appData.Values.Count > 0)
            {
                if (appData.Values.ContainsKey(AuthConstants.AccessToken))
                {
                    appData.Values.Remove(AuthConstants.AccessToken);
                }

                if (appData.Values.ContainsKey(AuthConstants.AuthenticationToken))
                {
                    appData.Values.Remove(AuthConstants.AuthenticationToken);
                }

                if (appData.Values.ContainsKey(AuthConstants.RefreshToken))
                {
                    appData.Values.Remove(AuthConstants.RefreshToken);
                }

                if (appData.Values.ContainsKey(AuthConstants.Expires))
                {
                    appData.Values.Remove(AuthConstants.Expires);
                }

                if (appData.Values.ContainsKey(AuthConstants.Scope))
                {
                    appData.Values.Remove(AuthConstants.Scope);
                }

                if (appData.Values.ContainsKey(AuthConstants.UserStatus))
                {
                    appData.Values.Remove(AuthConstants.UserStatus);
                }
            }
        }

        public LiveConnectSession LoadSession(LiveAuthClient authClient)
        {
            return null;
        }

        public void SaveSession(LiveConnectSession session)
        {
        }
    }
}
