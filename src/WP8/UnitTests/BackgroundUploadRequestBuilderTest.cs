// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
