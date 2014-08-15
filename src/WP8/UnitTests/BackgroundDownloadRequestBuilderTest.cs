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

namespace Microsoft.Live.WP8.UnitTests
{
    using System;

    using Microsoft.Live.Phone;
    using Microsoft.Phone.BackgroundTransfer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundDownloadRequestBuilderTest
    {
        [TestMethod]
        public void TestCreateBackgroundTransferRequestForDownload()
        {
            var requestUri = new Uri("https://apis.live.net/v5.0/foo/bar");
            var downloadLocation = new Uri(@"\shared\transfers\MyFile.xml", UriKind.RelativeOrAbsolute);
            const string accessToken = "myAccessToken";
            const TransferPreferences transferPreferences = TransferPreferences.AllowCellularAndBattery;

            var backgroundDownloadRequestBuilder = new BackgroundDownloadRequestBuilder
            {
                RequestUri = requestUri,
                AccessToken = accessToken,
                DownloadLocationOnDevice = downloadLocation,
                TransferPreferences = transferPreferences
            };

            BackgroundTransferRequest request = backgroundDownloadRequestBuilder.Build();

            Assert.AreEqual(requestUri, request.RequestUri, "request.RequestUri was not set properly.");

            Assert.AreEqual(
                downloadLocation, 
                request.DownloadLocation, 
                "request.DownloadLocationOnDevice was not set properly.");

            Assert.AreEqual(request.Tag, BackgroundTransferHelper.Tag);

            Assert.AreEqual(
                "bearer " + accessToken, 
                request.Headers["Authorization"], 
                "Authorization header was not set properly.");

            Assert.AreEqual(
                Platform.GetLibraryHeaderValue(),
                request.Headers["X-HTTP-Live-Library"],
                "Library Header not set properly.");

            Assert.AreEqual(transferPreferences, request.TransferPreferences);
        }
    }
}
