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

namespace Microsoft.Live.UnitTest.Phone.Operations
{
    using System;

    using Microsoft.Live.Phone.Operations;
    using Microsoft.Live.UnitTest.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundDownloadOperationTest
    {
        [TestMethod]
        public void TestAddNewRequestToBackgroundTransferService()
        {
            var requestUri = new Uri("http://apis.live.net/v5.0/me/skydrive");
            var locationOnDevice = new Uri("/shared/transfers/new_file.txt", UriKind.RelativeOrAbsolute);
            const string accessToken = "accessToken";
            var backgroundTransferService = new MockBackgroundTransferService();

            var builder = new BackgroundDownloadOperation.Builder()
            {
                AccessToken = accessToken,
                BackgroundTransferService = backgroundTransferService,
                DownloadLocationOnDevice = locationOnDevice,
                RequestUri = requestUri,
                BackgroundTransferPreferences = BackgroundTransferPreferences.AllowCellularAndBattery
            };

            BackgroundDownloadOperation operation = builder.Build();
            operation.ExecuteAsync();

            Assert.IsTrue(
                backgroundTransferService.RequestHasBeenAdded(),
                "A BackgroundTransferRequest was NOT added to the BackgroundTransferService.");
        }
    }
}
