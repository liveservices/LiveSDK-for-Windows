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

namespace Microsoft.Live.Phone
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// The concrete implementation of the BackgroundTransferService.
    /// The main reason why this class and its interface exists is for Dependency Injection (i.e., Unit Tests).
    /// </summary>
    internal class PhoneBackgroundTransferService : IBackgroundTransferService
    {

        #region Fields

        private static PhoneBackgroundTransferService instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor for singleton.
        /// </summary>
        private PhoneBackgroundTransferService()
        {
        }

        #endregion

        #region Properties

        public static PhoneBackgroundTransferService Instance
        {
            get { return instance ?? (instance = new PhoneBackgroundTransferService()); }
        }

        #endregion

        #region Methods

        public void Add(BackgroundTransferRequest request)
        {
            BackgroundTransferService.Add(request);
        }

        public void FindAllLiveSdkRequests(ICollection<BackgroundTransferRequest> matchingRequests)
        {
            foreach (BackgroundTransferRequest request in BackgroundTransferService.Requests)
            {
                // We only care about Transfers that we added.
                // Transfers we added are marked with our tag.
                if (BackgroundTransferHelper.BelongsToLiveSdk(request))
                {
                    matchingRequests.Add(request);
                }
            }
        }

        public void Remove(BackgroundTransferRequest request)
        {
            BackgroundTransferService.Remove(request);
        }

        #endregion
    }
}
