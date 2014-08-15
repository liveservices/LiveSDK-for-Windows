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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Live;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestCases : SilverlightTest
    {
        LiveAuthClient authClient;
        LiveConnectClient apiClient;

        [TestInitialize()]
        public void MyTestInitialize() 
        {
            this.authClient = new LiveAuthClient(TestAuthClient.ClientId);
            this.authClient.AuthClient = new TestAuthClient(this.authClient);
            this.authClient.AuthClient.CloseSession();
            WebRequestFactory.Current = new TestWebRequestFactory();
        }
        
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            this.authClient = null;
            this.apiClient = null;
        }

        [TestMethod]
        [Asynchronous]
        [Description("First time login and consent")]
        public async void LoginNoSession()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                Assert.IsNotNull(this.authClient.Session, "Session is null.");
                Assert.IsTrue(this.authClient.Session.IsValid, "Session is not valid.");
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Login with refresh token")]
        public async void LoginWithSession()
        {
            try
            {
                LiveConnectSession savedSession = new LiveConnectSession(this.authClient);
                savedSession.RefreshToken = TestAuthClient.FakeOldRefreshToken;

                this.authClient.AuthClient.SaveSession(savedSession);

                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveConnectSession session = this.authClient.Session;
                Assert.IsNotNull(session, "Session is null.");
                Assert.IsTrue(session.IsValid, "Session is not valid.");

                Assert.IsTrue(session.AccessToken == TestAuthClient.FakeAccessToken, "AccessToken is incorrect.");
                Assert.IsTrue(session.RefreshToken == TestAuthClient.FakeRefreshToken, "RefreshToken is incorrect.");
                Assert.IsTrue(new List<string>(session.Scopes).Count == TestAuthClient.Scopes.Length, "Scope count is incorrect.");
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Get me profile")]
        public async void GetMe()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);
                LiveOperationResult result = await this.apiClient.Get("me");
                ValidateMe(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Error: Put me profile")]
        public async void PutMe()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                await this.apiClient.Put("me", ResponseJson.Me);

                Assert.Fail("Expect error: request_method_invalid.");
            }
            catch (LiveConnectException error)
            {
                ValidateError(error, "request_method_invalid");
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Get the contacts collection.")]
        public async void GetContacts()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Get("/me/contacts/");
                ValidateContacts(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Get one contact info.")]
        public async void GetContac()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Get("contact.1e7d626c000000000000000000000000");
                ValidateContact(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Create a new contact.")]
        public async void CreateContact()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Post("me/contacts", ResponseJson.Contact);
                ValidateContact(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Update a new contact.")]
        public async void UpdateContact()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Put("contact.1e7d626c000000000000000000000000", ResponseJson.Contact);
                ValidateContact(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Delete a contact.")]
        public async void DeleteContact()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Delete("contact.1e7d626c000000000000000000000000");
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Result.Count == 0);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verify state object is passed through correctly.")]
        public async void VerifyStateObject()
        {
            string state = "some state to be passed.?~^;'*&#_@*$&%";

            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                this.apiClient.GetCompleted +=
                    (object sender, LiveOperationCompletedEventArgs args) =>
                    {
                        Assert.IsNotNull(args);
                        Assert.IsTrue(state.Equals(args.UserState as string, StringComparison.Ordinal));

                        EnqueueTestComplete();
                    };


                this.apiClient.GetAsync("me", state);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Get the files collection.")]
        public async void GetFiles()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Get("/me/skydrive/files");
                ValidateFiles(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Get a file.")]
        public async void GetFile()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Get("file.0bee3874468ef5b6.BEE3874468EF5B6!765");
                ValidateFile(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Copy a file.")]
        public async void CopyFile()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Copy("file.0bee3874468ef5b6.BEE3874468EF5B6!765", "folder.0bee3874468ef5b6.BEE3874468EF5B6!104");
                ValidateFile(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        [TestMethod]
        [Asynchronous]
        [Description("Move a file.")]
        public async void MoveFile()
        {
            try
            {
                await this.InitializeAuthClient(TestAuthClient.Scopes);

                LiveOperationResult result = await this.apiClient.Move("file.0bee3874468ef5b6.BEE3874468EF5B6!765", "folder.0bee3874468ef5b6.BEE3874468EF5B6!104");
                ValidateFile(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
            finally
            {
                EnqueueTestComplete();
            }
        }

        private async Task InitializeAuthClient(string[] scopes)
        {
            LiveLoginResult loginResult = await this.authClient.Initialize();
            if (loginResult.Status == LiveConnectSessionStatus.Connected)
            {
                this.apiClient = new LiveConnectClient(loginResult.Session);
            }
            else
            {
                loginResult = await this.authClient.Login(scopes);
                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    this.apiClient = new LiveConnectClient(loginResult.Session);
                }
                else
                {
                    throw new InvalidOperationException("LiveAuthClient failed to login. The status is " + loginResult.Status.ToString());
                }
            }
        }

        private static void ValidateMe(LiveOperationResult result)
        {
            Assert.IsNotNull(result, "ValidateMe failed.  result is null.");
            Assert.IsNotNull(result.Result, "ValidateMe failed.  result.Result is null.");
            Assert.IsTrue(!string.IsNullOrEmpty(result.RawResult), "ValidateMe failed.  result.RawResult is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(result.Result["id"] as string), "ValidateMe failed.  'id' is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(result.Result["first_name"] as string), "ValidateMe failed.  'first_name' is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(result.Result["last_name"] as string), "ValidateMe failed.  'last_name' is null or empty.");
        }

        private static void ValidateError(LiveConnectException error, string expectedCode)
        {
            Assert.IsTrue(error.ErrorCode.Equals(expectedCode, StringComparison.Ordinal), "Unexpected error code.  Expect: " + expectedCode + " Actual: " + error.ErrorCode);
            Assert.IsTrue(!string.IsNullOrEmpty(error.Message));
        }

        private static void ValidateContacts(LiveOperationResult result)
        {
            Assert.IsNotNull(result, "ValidateContacts failed.  result is null.");
            Assert.IsNotNull(result.Result, "ValidateContacts failed.  result.Result is null.");
            var data = result.Result["data"] as IList;
            Assert.IsNotNull(data, "ValidateContacts failed.  'data' is null.");
            Assert.AreEqual(data.Count, 3, "ValidateContacts failed.  'data' count is incorrect.");

            foreach (object value in data)
            {
                var contactProperties = value as IDictionary<string, object>;
                ValidateContactProperties(contactProperties);
            }
        }

        private static void ValidateContact(LiveOperationResult result)
        {
            Assert.IsNotNull(result, "ValidateContact failed.  result is null.");
            Assert.IsNotNull(result.Result, "ValidateContact failed.  result.Result is null.");

            ValidateContactProperties(result.Result);
        }

        private static void ValidateContactProperties(IDictionary<string, object> contactProperties)
        {
            Assert.IsNotNull(contactProperties, "ValidateContactProperties failed.  contactProperties is null.");
            Assert.IsTrue(!string.IsNullOrEmpty(contactProperties["id"] as string), "ValidateContactProperties failed.  'id' is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(contactProperties["first_name"] as string), "ValidateContactProperties failed.  'first_name' is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(contactProperties["last_name"] as string), "ValidateContactProperties failed.  'last_name' is null or empty.");

            var emailHashes = contactProperties["email_hashes"] as IList;
            Assert.IsNotNull(emailHashes, "ValidateContactProperties failed.  'email_hashes' is null.");
            Assert.AreNotEqual(emailHashes.Count, 0, "ValidateContactProperties failed.  'email_hashes' count is incorrect.");

            foreach (string hash in emailHashes)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(hash), "ValidateContactProperties failed.  'hash' is null or empty.");
            }
        }

        private static void ValidateFiles(LiveOperationResult result)
        {
            Assert.IsNotNull(result, "ValidateFiles failed.  result is null.");
            Assert.IsNotNull(result.Result, "ValidateFiles failed.  result.Result is null.");
            var data = result.Result["data"] as IList;
            Assert.IsNotNull(data, "ValidateFiles failed.  'data' is null.");
            Assert.AreEqual(data.Count, 6, "ValidateFiles failed.  'data' count is incorrect.");

            foreach (object value in data)
            {
                var fileProperties = value as IDictionary<string, object>;
                Assert.IsTrue(!string.IsNullOrEmpty(fileProperties["id"] as string), "ValidateFiles failed.  'id' is null or empty.");

                string type = fileProperties["type"] as string;
                Assert.IsTrue(!string.IsNullOrEmpty(type), "ValidateFiles failed.  'type' is null or empty.");

                if (string.CompareOrdinal(type, "folder") == 0 || string.CompareOrdinal(type, "album") == 0)
                {
                    Assert.IsTrue(!string.IsNullOrEmpty(fileProperties["name"] as string), "ValidateFiles failed.  'name' is null or empty.");
                    Assert.IsTrue(!string.IsNullOrEmpty(fileProperties["upload_location"] as string), "ValidateFiles failed.  'upload_location' is null or empty.");
                }
                else
                {
                    ValidateFileProperties(fileProperties);
                }
            }
        }

        private static void ValidateFile(LiveOperationResult result)
        {
            Assert.IsNotNull(result, "ValidateFiles failed.  result is null.");
            Assert.IsNotNull(result.Result, "ValidateFiles failed.  result.Result is null.");

            ValidateFileProperties(result.Result);
        }

        private static void ValidateFileProperties(IDictionary<string, object> fileProperties)
        {
            Assert.IsNotNull(fileProperties, "ValidateFileProperties failed.  fileProperties is null.");
            Assert.IsTrue(!string.IsNullOrEmpty(fileProperties["name"] as string), "ValidateFileProperties failed.  'name' is null or empty.");
            Assert.IsTrue(!string.IsNullOrEmpty(fileProperties["upload_location"] as string), "ValidateFileProperties failed.  'upload_location' is null or empty.");

            string link = fileProperties["link"] as string;
            Assert.IsTrue(!string.IsNullOrEmpty(link), "ValidateFileProperties failed.  'link' is null or empty.");
            Uri linkUrl = new Uri(link, UriKind.Absolute);

            DateTime whenTaken;
            Assert.IsTrue(DateTime.TryParse(fileProperties["when_taken"] as string, out whenTaken));

            var images = fileProperties["images"] as IList;
            Assert.IsNotNull(images, "ValidateFileProperties failed.  'images' is null.");
            Assert.AreEqual(images.Count, 4, "ValidateFileProperties failed.  'images' count is incorrect.");

            var image = images[0] as IDictionary<string, object>;
            Assert.IsNotNull(image, "ValidateFileProperties failed.  'image' is null.");
            Assert.IsTrue(!string.IsNullOrEmpty(image["source"] as string), "ValidateFileProperties failed.  'image.source' is null or empty.");
        }
    }
}
