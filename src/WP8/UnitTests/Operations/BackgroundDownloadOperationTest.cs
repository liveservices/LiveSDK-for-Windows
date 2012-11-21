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
