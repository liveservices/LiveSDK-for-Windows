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
