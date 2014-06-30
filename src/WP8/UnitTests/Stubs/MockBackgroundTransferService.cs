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

namespace Microsoft.Live.UnitTest.Stubs
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Live.Phone;
    using Microsoft.Phone.BackgroundTransfer;

    public class MockBackgroundTransferService : IBackgroundTransferService
    {
        private BackgroundTransferRequest request;

        
        public void Add(BackgroundTransferRequest request)
        {
            this.request = request;
        }

        public void FindAllLiveSdkRequests(ICollection<BackgroundTransferRequest> matchingRequests)
        {
        }

        public bool RequestHasBeenAdded()
        {
            return this.request != null;
        }

        public void Remove(BackgroundTransferRequest request)
        {
        }
    }
}
