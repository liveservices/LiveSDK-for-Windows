namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;

    internal interface IServerResponseReaderObserver
    {
        void OnSuccessResponse(IDictionary<string, object> result, string rawResult);
        void OnErrorResponse(string code, string message);
        void OnInvalidJsonResponse(FormatException exception);
    }
}
