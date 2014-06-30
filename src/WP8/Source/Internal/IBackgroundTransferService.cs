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
    /// This interface is responsible for adding BackgroundTransferRequests to a service to be serviced.
    /// It is responsible for removing BackgroundTransferRequests that have been or are being serviced.
    /// It is responsible for returning all the BackgroundTransferRequests that belong to the Live SDK.
    /// The main reason why this interface exists is for Dependency Injection (i.e., Unit Tests).
    /// </summary>
    internal interface IBackgroundTransferService
    {
        /// <summary>
        /// Adds a BackgroundTransferRequest to the service, so it can be serviced.
        /// </summary>
        /// <param name="request"></param>
        void Add(BackgroundTransferRequest request);

        /// <summary>
        /// Adds all requests that were created by the Live SDK to the matchingRequests collection.
        /// </summary>
        /// <param name="matchingRequests"></param>
        void FindAllLiveSdkRequests(ICollection<BackgroundTransferRequest> matchingRequests);

        /// <summary>
        /// Removes a BackgroundTransferRequest from
        /// </summary>
        /// <param name="request"></param>
        void Remove(BackgroundTransferRequest request);
    }
}
