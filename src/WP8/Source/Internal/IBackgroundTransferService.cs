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
