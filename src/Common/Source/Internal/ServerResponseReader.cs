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
