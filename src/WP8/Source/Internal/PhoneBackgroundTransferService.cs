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
