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
    public class BackgroundTransferHelperTest
    {
        [TestMethod]
        public void TestGetTransferPreferencesAllBackgroundTransferPreferencesEnumsAreHandled()
        {
            Assert.AreEqual(
                Enum.GetValues(typeof(TransferPreferences)).Length,
                Enum.GetValues(typeof (BackgroundTransferPreferences)).Length,
                "There should be the same number of values for TransferPreferences and BackgroundTransferPreferences");
        }

        [TestMethod]
        public void TestGetTransferPreferencesNone()
        {
            Assert.AreEqual(
                TransferPreferences.None, 
                BackgroundTransferHelper.GetTransferPreferences(BackgroundTransferPreferences.None));
        }

        [TestMethod]
        public void TestGetTransferPreferencesAllowBattery()
        {
            Assert.AreEqual(
                TransferPreferences.AllowBattery, 
                BackgroundTransferHelper.GetTransferPreferences(BackgroundTransferPreferences.AllowBattery));
        }

        [TestMethod]
        public void TestGetTransferPreferencesAllowCellular()
        {
            Assert.AreEqual(
                TransferPreferences.AllowCellular, 
                BackgroundTransferHelper.GetTransferPreferences(BackgroundTransferPreferences.AllowCellular));
        }

        [TestMethod]
        public void TestGetTransferPreferencesAllowCellularAndBattery()
        {
            Assert.AreEqual(
                TransferPreferences.AllowCellularAndBattery, 
                BackgroundTransferHelper.GetTransferPreferences(BackgroundTransferPreferences.AllowCellularAndBattery));
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithForwardSlashes()
        {
            var location = new Uri("/shared/transfers/myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithForwardSlashesAndNoFileName()
        {
            var location = new Uri("/shared/transfers/", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithForwardSlashesAndNoLeadingSlash()
        {
            var location = new Uri("shared/transfers/myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithBackwardSlashes()
        {
            var location = new Uri(@"\shared\transfers\myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithBackwardSlashesAndNoFileName()
        {
            var location = new Uri(@"\shared\transfers\", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithBackwardSlashesAndNoLeadingSlash()
        {
            var location = new Uri(@"shared\transfers\myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithMixedSlashes()
        {
            var location = new Uri(@"\shared/transfers\myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);

            location = new Uri(@"/shared\transfers/myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersDirectoryWithMixedSlashesAndNoLeadingSlash()
        {
            var location = new Uri(@"shared/transfers\myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);

            location = new Uri(@"shared\transfers/myFile.txt", UriKind.Relative);
            Assert.IsTrue(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Did not accept valid location: " + location);
        }

        [TestMethod]
        public void TestNotInSharedTransfersDirectory()
        {
            var location = new Uri(@"/notshared/transfers/myFile.txt", UriKind.Relative);
            Assert.IsFalse(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Accepted an invalid location: " + location);

            location = new Uri(@"/shared/nottransfers/myFile.txt", UriKind.Relative);
            Assert.IsFalse(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Accepted an invalid location: " + location);
        }

        [TestMethod]
        public void TestSharedTransfersInSubPath()
        {
            var location = new Uri(@"/blah/shared/transfers/myFile.txt", UriKind.Relative);
            Assert.IsFalse(
                BackgroundTransferHelper.IsRootedInSharedTransfers(location),
                "Accepted an invalid location: " + location);
        }

        [TestMethod]
        public void TestAccept2xxStatusCode()
        {
            for (long statusCode = 200; statusCode < 300; statusCode++)
            {
                Assert.IsTrue(
                    BackgroundTransferHelper.IsSuccessfulStatusCode(statusCode),
                    "Failed to accept a failed statusCode: " + statusCode);
            }
        }

        [TestMethod]
        public void TestRejectNon2xxStatusCode()
        {
            for (long statusCode = 0; statusCode < 600; statusCode++)
            {
                if (statusCode >= 200 && statusCode < 300)
                {
                    statusCode = 300;
                    continue;
                }

                Assert.IsFalse(
                    BackgroundTransferHelper.IsSuccessfulStatusCode(statusCode),
                    "Accepted an invalid statusCode: " + statusCode);
            }
        }

        [TestMethod]
        public void TestIsDownloadRequest()
        {
            var request = new BackgroundTransferRequest(new Uri("/shared/transfers/file.txt", UriKind.Relative))
                              {Method = "get"};

            Assert.IsTrue(
                BackgroundTransferHelper.IsDownloadRequest(request),
                "Did not recognize a valid download request");


            request.Method = "post";
            Assert.IsFalse(
                BackgroundTransferHelper.IsDownloadRequest(request),
                "Thought an upload request was a download request.");
        }

        [TestMethod]
        public void TestIsUploadRequest()
        {
            var request = new BackgroundTransferRequest(new Uri("/shared/transfers/file.txt", UriKind.Relative))
                              {Method = "post"};

            Assert.IsTrue(
                BackgroundTransferHelper.IsUploadRequest(request),
                "Did not recognize a valid upload request");


            request.Method = "get";
            Assert.IsFalse(
                BackgroundTransferHelper.IsUploadRequest(request),
                "Thought a download request was a upload request.");
        }
    }
}
