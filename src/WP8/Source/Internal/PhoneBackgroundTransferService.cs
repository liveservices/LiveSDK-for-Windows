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
