namespace Microsoft.Live.Phone
{
    using System;
    using System.Collections.Generic;

    internal interface IBackgroundUploadResponseHandlerObserver
    {
        void OnError(Exception exception);
        void OnErrorResponse(string code, string message);
        void OnSuccessResponse(IDictionary<string, object> result, string rawResult);
    }
}
