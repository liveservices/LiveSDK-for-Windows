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
using System.Transactions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Microsoft.Live.Web.Samples.ConnectAppUserWithMSAccount.Filters;
using Microsoft.Live.Web.Samples.ConnectAppUserWithMSAccount.Models;

namespace Microsoft.Live.Web.Samples.ConnectAppUserWithMSAccount.Controllers
{
    public enum MSAccountStatus
    {
        None,
        Connected,
        NotConnected,
        ConnectedWithError
    }

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller, IRefreshTokenHandler
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
        private MSAccountStatus msAccountStatus = MSAccountStatus.None;

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            this.MSAuthClient.ClearSession(this.HttpContext);

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        public async Task<ActionResult> ConnectMicrosoft()
        {
            if (ClientId.Contains('%') || ClientSecret.Contains('%') || ClientDomain.Contains('%'))
            {
                throw new ArgumentException("Update the ClientId, ClientSecret, and ClientDomain with your app client Id, client secret and domain values that you get from https://account.live.com/developers/applications.");
            }

            await InitMSAccountAsync();

            ActionResult view = await this.BuildMSAccountPageView(
                new string[] { "wl.signin", "wl.offline_access" }, 
                AccountDataContent.Basic);

            return view;
        }

        [Authorize]
        public async Task<ActionResult> ConnectCallback()
        {
            try
            {
                LiveLoginResult result = await this.MSAuthClient.ExchangeAuthCodeAsync(this.HttpContext, this.CurrentPath);
            }
            catch (LiveAuthException ex)
            {
            }

            ActionResult view = await this.BuildMSAccountPageView(
                new string[] { "wl.signin", "wl.offline_access" },
                AccountDataContent.Basic);

            return view;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisconnectMicrosoft()
        {
            if (this.MSAccountStatus != Controllers.MSAccountStatus.NotConnected)
            {
                // Remove the account association
                using (UsersContext db = new UsersContext())
                {
                    MSAccount msAccount = db.MSAccounts.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                    if (msAccount != null)
                    {
                        db.MSAccounts.Remove(msAccount);
                        db.SaveChanges();
                    }
                }

                this.msAccountStatus = MSAccountStatus.NotConnected;
            }

            this.MSAuthClient.ClearSession(this.HttpContext);

            this.Response.Redirect(this.SiteRootPath + "account/connectmicrosoft");
            return null;
        }

        public async Task<ActionResult> ShowContacts()
        {
            await InitMSAccountAsync();

            string requiredScope = "wl.basic";
            string[] scopes = new string[] { "wl.signin", "wl.offline_access", requiredScope };
            LiveConnectSession session = this.MSAuthClient.Session;
            bool hasPermission = session != null && session.Scopes.Contains<string>(requiredScope);
            Dictionary<string, string> options = new Dictionary<string,string>();
            options["state"] = "show contacts";
            if (!hasPermission)
            {
                string authorizeUrl = this.MSAuthClient.GetLoginUrl(
                    scopes,
                    this.ShowContactsPath,
                    options);

                this.Response.Redirect(authorizeUrl);
                return null;
            }

            return await this.BuildMSAccountPageView(scopes, AccountDataContent.Basic | AccountDataContent.Contacts);
        }

        public ActionResult AjaxClient()
        {
            return View("AjaxClient", this.JSClientModel);
        }

        public async Task<ActionResult> AjaxCallback()
        {
            await this.InitMSAccountAsync();
            return View("AjaxCallback", this.JSClientModel);
        }

        private LiveAuthClient MSAuthClient
        {
            get
            {
                if (this.liveAuthClient == null)
                {
                    IDictionary<int, string> secretMap = new Dictionary<int, string>();
                    secretMap.Add(ClientSecretKey, ClientSecret);
                    this.liveAuthClient = new LiveAuthClient(ClientId, secretMap, null, this);
                }

                return this.liveAuthClient;
            }
        }

        private MSAccountStatus MSAccountStatus
        {
            get
            {
                if (this.msAccountStatus == Controllers.MSAccountStatus.None)
                {
                    using (UsersContext db = new UsersContext())
                    {
                        MSAccount msAccount = db.MSAccounts.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                        this.msAccountStatus = msAccount != null ? MSAccountStatus.Connected : MSAccountStatus.NotConnected;
                    }
                }

                if (this.msAccountStatus == MSAccountStatus.Connected)
                {
                    LiveConnectSession session = this.MSAuthClient.Session;
                    if (session == null || session.Expires < DateTimeOffset.UtcNow)
                    {
                        this.msAccountStatus = MSAccountStatus.ConnectedWithError;
                    }
                }

                return this.msAccountStatus;
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

        private string ShowContactsPath
        {
            get
            {
                return this.SiteRootPath + "account/showcontacts";
            }
        }
        
        private string WLJSPath
        {
            get
            {
                return "https://js.live.net/v5.0/wl.js";
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

        private JSClientModel JSClientModel
        {
            get
            {
                var model = new JSClientModel()
                {
                    JSPath = this.WLJSPath,
                    ClientId = ClientId,
                    Scope = "wl.signin wl.offline_access",
                    RedirectUri = this.AjaxCallbackPath
                };

                return model;
            }
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        private async Task<byte[]> GetProfileImageData(LiveConnectClient client)
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

        private async Task PopulateMSAccountData(MSAccountPageModel model, AccountDataContent content)
        {
            string errorText = null;
            LiveConnectClient client = new LiveConnectClient(this.MSAuthClient.Session);
            LiveOperationResult meResult = await QueryUserData(client, "me");
            if (meResult != null)
            {
                dynamic meInfo = meResult.Result;
                model.Name = meInfo.name;
                model.ProfileImage = await GetProfileImageData(client);
            }
            else
            {
                errorText = "There was an error while retrieving the user's information. Please try again later.";
            }

            if (errorText == null && 
                (content & AccountDataContent.Contacts) == AccountDataContent.Contacts)
            {
                LiveOperationResult contactsResult = await QueryUserData(client, "me/contacts");
                if (contactsResult != null)
                {
                    dynamic contacts = contactsResult.Result;
                    model.Contacts = contacts.data;
                }
                else
                {
                    errorText = "There was an error while retrieving the user's contacts data. Please try again later.";
                }
            }

            model.Error = errorText;
        }

        private async Task<LiveOperationResult> QueryUserData(LiveConnectClient client, string path)
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

        private async Task InitMSAccountAsync()
        {
            try
            {
                LiveLoginResult result = await this.MSAuthClient.InitializeWebSessionAsync(
                    this.HttpContext,
                    this.CurrentPath);
            }
            catch (LiveAuthException ex)
            {
            }
        }

        private async Task<ActionResult> BuildMSAccountPageView(string[] scopes, AccountDataContent content)
        {
            MSAccountStatus accountStatus = this.MSAccountStatus;

            MSAccountPageModel model = new MSAccountPageModel()
            {
                Status = accountStatus,
                AjaxClientURL = this.AjaxClientPath,
                ShowContactsURL = this.ShowContactsPath
            };

            if (accountStatus == MSAccountStatus.Connected)
            {
                await this.PopulateMSAccountData(model, content);
            }
            else
            {
                Dictionary<string, string> options = new Dictionary<string, string>();
                options["state"] = "connect";
                model.LoginUrl = this.MSAuthClient.GetLoginUrl(scopes, this.SiteRootPath + "account/connectcallback", options);
            }

            return View("MSAccountPage", model);
        }

        Task IRefreshTokenHandler.SaveRefreshTokenAsync(RefreshTokenInfo tokenInfo)
        {
            this.msAccountStatus = MSAccountStatus.Connected;
            return Task.Factory.StartNew(() =>
            {
                using (UsersContext db = new UsersContext())
                {
                    MSAccount msAccount = db.MSAccounts.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                    if (msAccount != null)
                    {
                        if (tokenInfo.UserId != msAccount.MSUserId)
                        {
                            throw new LiveAuthException("invalid_user", "The account Id from the ticket does not match the current connected user.");
                        }

                        msAccount.RefreshToken = tokenInfo.RefreshToken;
                        db.Entry(msAccount).State = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        // Insert name into the profile table
                        db.MSAccounts.Add(new MSAccount
                        {
                            MSUserId = tokenInfo.UserId,
                            RefreshToken = tokenInfo.RefreshToken,
                            UserName = this.User.Identity.Name
                        });
                    }

                    db.SaveChanges();
                }
            });
        }

        Task<RefreshTokenInfo> IRefreshTokenHandler.RetrieveRefreshTokenAsync()
        {
            return Task.Factory.StartNew<RefreshTokenInfo>(() =>
            {
                RefreshTokenInfo token = null;
                using (UsersContext db = new UsersContext())
                {
                    MSAccount msAccount = db.MSAccounts.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                    if (msAccount != null)
                    {
                        token = new RefreshTokenInfo(msAccount.RefreshToken, msAccount.MSUserId);
                        this.msAccountStatus = MSAccountStatus.Connected;
                    }
                }

                return token;
            });
        }
    }
}
