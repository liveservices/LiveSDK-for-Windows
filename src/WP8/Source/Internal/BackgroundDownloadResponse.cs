namespace Microsoft.Live.Phone
{
    using System;
    using System.Diagnostics;

    internal class BackgroundDownloadResponse
    {
        private readonly Uri locationOfResponesOnDevice;
        private readonly string userState;

        public BackgroundDownloadResponse(Uri locationOfResponseOnDevice, string userState)
        {
            Debug.Assert(locationOfResponseOnDevice != null);

            this.locationOfResponesOnDevice = locationOfResponseOnDevice;
            this.userState = userState;
        }

        public Uri LocationOfResponseOnDevice
        {
            get { return this.locationOfResponesOnDevice; }
        }

        public string UserState
        {
            get { return this.userState; }
        }
    }
}
