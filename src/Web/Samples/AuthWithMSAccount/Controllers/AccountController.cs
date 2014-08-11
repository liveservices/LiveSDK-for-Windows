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
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Live;
using Microsoft.Live.Web.Samples.AuthWithMSAccount.Models;

namespace Microsoft.Live.Web.Samples.AuthWithMSAccount.Controllers
{
    public class AccountController : Controller
    {
        [Flags]
        private enum AccountDataContent
        {
            Basic = 1,
            Contacts = 2,
        }

        private const string ClientId = "%YourAppClientId%"; // Your app client Id
        private const int ClientSecretKey = 0; // Your app secret key (index)
        private const string ClientSecret = "%YourAppSecret%"; // Your app secret
        private const string ClientDomain = "%YourAppDomain%"; // Your app site domain, e.g. www.foo.com

        private LiveAuthClient liveAuthClient;

        //
        // GET: /Home/

        public async Task<ActionResult> Index()
        {
            if (ClientId.Contains('%') || ClientSecret.Contains('%') || ClientDomain.Contains('%'))
            {
                throw new ArgumentException("Update the ClientId, ClientSecret, and ClientDomain with your app client Id, client secret and domain values that you get from https://account.live.com/developers/applications.");
            }

            try
            {
                LiveLoginResult loginStatus = await this.AuthClient.InitializeWebSessionAsync(this.HttpContext, this.CurrentPath);
                switch (loginStatus.Status)
                {
                    case LiveConnectSessionStatus.Expired:
                        string reAuthUrl = this.GetLoginUrl(new string[] { "wl.signin" }, this.CurrentPath);
                        this.Response.Redirect(reAuthUrl);
                        return null;
                    case LiveConnectSessionStatus.Connected:
                        return await GetAccountDataView(AccountDataContent.Basic);
                }
            }
            catch (LiveAuthException ex)
            {   
                // Log errors
            }

            return this.GetLoggedOutView();
        }

        public ActionResult Logout()
        {
            this.AuthClient.ClearSession(this.HttpContext);
            return this.GetLoggedOutView();
        }

        public async Task<ActionResult> AuthHandler()
        {
            string redirectPath;
            try
            {
                LiveLoginResult result = await this.AuthClient.ExchangeAuthCodeAsync(this.HttpContext, this.CurrentPath);
                redirectPath = result.State;
            }
            catch (LiveAuthException ex)
            {
                redirectPath = ex.State;
            }

            this.Response.Redirect(redirectPath);

            return null;
        }

        public async Task<ActionResult> ShowContacts()
        {
            try
            {
                var scopes = new string[] { "wl.signin", "wl.basic" };
                LiveLoginResult loginStatus = await this.AuthClient.InitializeWebSessionAsync(
                    this.HttpContext,
                    this.CurrentPath,
                    scopes);

                switch (loginStatus.Status)
                {
                    case LiveConnectSessionStatus.Expired:
                        {
                            string reAuthUrl = this.GetLoginUrl(scopes, this.CurrentPath);
                            this.Response.Redirect(reAuthUrl);
                            return null;
                        }
                    case LiveConnectSessionStatus.Connected:
                        {
                            return await this.GetAccountDataView(AccountDataContent.Basic | AccountDataContent.Contacts);
                        }
                    default:
                        {
                            string authorizeUrl = this.GetLoginUrl(
                                scopes,
                                ShowContactsUrl);
                            this.Response.Redirect(authorizeUrl);
                            return null;
                        }
                }
            }
            catch (LiveAuthException ex)
            {
                // Log errors
            }

            return this.GetLoggedOutView();
        }

        public ActionResult AjaxClient()
        {
            return View("AjaxClient", this.JSClientModel);
        }

        public async Task<ActionResult> AjaxCallback()
        {
            try
            {
                LiveLoginResult loginStatus = await this.AuthClient.InitializeWebSessionAsync(this.HttpContext, this.CurrentPath);
                if (loginStatus.Status == LiveConnectSessionStatus.Expired)
                {
                    string reAuthUrl = this.AuthClient.GetLoginUrl(new string[] { "wl.signin" }, this.CurrentPath);
                    this.Response.Redirect(reAuthUrl);
                    return null;
                }
            }
            catch (LiveAuthException)
            {

            }
            
            return View("AjaxCallback", this.JSClientModel);
        }

        private JSClientModel JSClientModel
        {
            get
            {
                var model = new JSClientModel()
                {
                    JSPath = this.WLJSPath,
                    ClientId = ClientId,
                    Scope = "wl.signin wl.basic wl.skydrive",
                    RedirectUri = this.AjaxCallbackPath
                };

                return model;
            }
        }

        private string WLJSPath
        {
            get
            {
                return "https://js.live.net/v5.0/wl.js";
            }
        }

        private string AuthHandlerPath
        {
            get
            {
                return this.SiteRootPath + "account/authhandler";
            }
        }

        private string AjaxClientPath
        {
            get
            {
                return this.SiteRootPath + "account/ajaxclient";
            }
        }

        private string AjaxCallbackPath
        {
            get
            {
                return this.SiteRootPath + "account/ajaxcallback";
            }
        }

        private string ShowContactsUrl
        {
            get
            {
                return this.SiteRootPath + "account/showcontacts";
            }
        }

        private string SiteRootPath
        {
            get
            {
                string localhost = "localhost";
                Uri currentUri = this.Request.Url;
                string currentUrl = currentUri.GetLeftPart(UriPartial.Authority);
                if (currentUri.Host == localhost)
                {
                    currentUrl = currentUri.GetLeftPart(UriPartial.Authority).Replace(localhost, ClientDomain);
                }

                if (!currentUrl.EndsWith("/"))
                {
                    currentUrl += "/";
                }

                return currentUrl;
            }
        }

        private string CurrentPath
        {
            get
            {
                return this.SiteRootPath + this.Request.Path.TrimStart('/');
            }
        }

        private LiveAuthClient AuthClient
        {
            get
            {
                if (this.liveAuthClient == null)
                {
                    IDictionary<int, string> secretMap = new Dictionary<int, string>();
                    secretMap.Add(ClientSecretKey, ClientSecret);
                    this.liveAuthClient = new LiveAuthClient(ClientId, secretMap, null);
                }

                return this.liveAuthClient;
            }
        }

        private static async Task<LiveOperationResult> QueryUserData(LiveConnectClient client, string path)
        {
            try
            {
                return await client.GetAsync(path);
            }
            catch
            {
            }

            return null;
        }

        private async Task<ActionResult> GetAccountDataView(AccountDataContent content)
        {
            string errorText = null;
            string logoutUrl = this.AuthClient.GetLogoutUrl(this.SiteRootPath + "account/logout");
            var accountData = new AccountDataModel()
            {
                LogoutURL = logoutUrl,
                ShowContactsURL = ShowContactsUrl,
                HomeURL = this.SiteRootPath
            };

            LiveConnectClient client = new LiveConnectClient(this.AuthClient.Session);
            LiveOperationResult meResult = await QueryUserData(client, "me");
            if (meResult != null)
            {
                dynamic meInfo = meResult.Result;
                accountData.Name = meInfo.name;

                accountData.ProfileImage = await GetProfileImageData(client);
            }
            else
            {
                errorText = "There was an error while retrieving the user's information. Please try again later.";
            }

            if (errorText == null && (content & AccountDataContent.Contacts) == AccountDataContent.Contacts)
            {
                LiveOperationResult contactsResult = await QueryUserData(client, "me/contacts");
                if (contactsResult != null)
                {
                    dynamic contacts = contactsResult.Result;
                    accountData.Contacts = contacts.data;
                }
                else
                {
                    errorText = "There was an error while retrieving the user's contacts data. Please try again later.";
                }
            }

            accountData.Error = errorText;
            return View("AccountData", accountData);
        }

        private ActionResult GetLoggedOutView()
        {
            return View("LoggedOut", new LoggedOutModel()
            {
                LoginURL = this.GetLoginUrl(new string[] { "wl.signin" }, this.SiteRootPath),
                AjaxClientURL = this.AjaxClientPath
            });
        }

        private string GetLoginUrl(IEnumerable<string> scopes, string callbackUrl)
        {
            var options = new Dictionary<string, string>();
            options["state"] = callbackUrl;
            string loginUrl = this.AuthClient.GetLoginUrl(scopes, this.AuthHandlerPath, options);

            return loginUrl;
        }

        private static async Task<byte[]> GetProfileImageData(LiveConnectClient client)
        {
            byte[] imgData = null;
            try
            {
                LiveDownloadOperationResult meImgResult = await client.DownloadAsync("me/picture");
                imgData = new byte[meImgResult.Stream.Length];
                await meImgResult.Stream.ReadAsync(imgData, 0, imgData.Length);
            }
            catch
            {
                // Failed to download image data.
            }

            return imgData;
        }
    }
}
