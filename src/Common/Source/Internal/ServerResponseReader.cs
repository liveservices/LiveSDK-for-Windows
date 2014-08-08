namespace Microsoft.Live
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Live.Operations;
    using Microsoft.Live.Serialization;

    /// <summary>
    /// Reads in the response from a server and calls back a method on the observer
    /// depending on what the response was like.
    /// </summary>
    internal class ServerResponseReader
    {
        private static ServerResponseReader instance;

        private ServerResponseReader()
        {
            // Private for singleton.
        }

        public static ServerResponseReader Instance
        {
            get { return instance ?? (instance = new ServerResponseReader()); }
        }

        public void Read(string response, IServerResponseReaderObserver observer)
        {
            using (var jsonReader = new JsonReader(response))
            {
                IDictionary<string, object> jsonObject;
                try
                {
                    jsonObject = jsonReader.ReadValue() as IDictionary<string, object>;
                }
                catch (FormatException fe)
                {
                    observer.OnInvalidJsonResponse(fe);
                    return;
                }

                if (jsonObject.ContainsKey(ApiOperation.ApiError))
                {
                    var errorObj = jsonObject[ApiOperation.ApiError] as IDictionary<string, object>;
                    var code = errorObj[ApiOperation.ApiErrorCode] as string;
                    var message = errorObj[ApiOperation.ApiErrorMessage] as string;
                    observer.OnErrorResponse(code, message);
                    return;
                }

                observer.OnSuccessResponse(jsonObject, response);
            }
        }
    }
}
