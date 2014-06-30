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
