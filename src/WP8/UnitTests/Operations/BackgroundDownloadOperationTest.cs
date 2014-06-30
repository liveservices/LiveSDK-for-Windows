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
