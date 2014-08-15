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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Phone.BackgroundTransfer;

    [TestClass]
    public class BackgroundUploadRequestBuilderTest
    {
        [TestMethod]
        public void TestCreateBackgroundTransferRequestForUpload()
        {
            const string accessToken = "accessToken";
            var requestUri = new Uri("https://apis.live.net/v5.0/me/skydrive/files");
            var uploadLocation = new Uri(@"\shared\transfers\myFile.txt", UriKind.RelativeOrAbsolute);
            var downloadLocation = new Uri(uploadLocation.OriginalString + ".json", UriKind.RelativeOrAbsolute);
            const TransferPreferences transferPreferences = TransferPreferences.AllowCellularAndBattery;

            var builder = new BackgroundUploadRequestBuilder()
            {
                AccessToken = accessToken,
                DownloadLocationOnDevice = downloadLocation,
                RequestUri = requestUri,
                UploadLocationOnDevice = uploadLocation,
                TransferPreferences = transferPreferences
            };

            BackgroundTransferRequest request = builder.Build();

            var expectedRequestUri = new Uri(requestUri.OriginalString + "?method=PUT");
            Assert.AreEqual(expectedRequestUri, request.RequestUri, "request.RequestUri was not set properly.");

            Assert.AreEqual(
                downloadLocation, 
                request.DownloadLocation, 
                "request.DownloadLocation was not set properly.");

            Assert.AreEqual(
                uploadLocation, 
                request.UploadLocation, 
                "request.UploadLocation was not set properly.");

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
