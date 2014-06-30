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

namespace Microsoft.Live.UnitTests
{
    using System;

    using Microsoft.Live.Operations;
    using Microsoft.Live.UnitTest;
    using Microsoft.Live.Win8.UnitTests.Stubs;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using Windows.Storage;

    [TestClass]
    public class TailoredDownloadOperationTest
    {

        // TODO: Test Attach, Execute, and Cancel

        [TestMethod]
        public void TestExecute()
        {
            WebRequestFactory.Current = new TestWebRequestFactory();
            var connectClient = new LiveConnectClient(new LiveConnectSession());
            var requestUrl = new Uri("http://foo.com");
            IStorageFile outputFile = new StorageFileStub();
            IProgress<LiveOperationProgress> progress = new Progress<LiveOperationProgress>();
            SynchronizationContextWrapper syncContext = SynchronizationContextWrapper.Current;

            var tailoredDownloadOperation =
                new TailoredDownloadOperation(connectClient, requestUrl, outputFile, progress, syncContext);
        }
    }
}
