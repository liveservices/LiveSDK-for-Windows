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
