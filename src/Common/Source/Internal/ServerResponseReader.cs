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
