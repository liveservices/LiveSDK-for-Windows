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

namespace Microsoft.Live.Phone
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;

    /// <summary>
    /// This class will asynchronously read the response from a background upload request.
    /// It will delete the response once it is done reading it.
    /// </summary>
    internal class BackgroundUploadResponseHandler : IBackgroundWorkerStrategy<Action>, IServerResponseReaderObserver
    {

        #region Fields

        private readonly Uri downloadLocation;
        private readonly IBackgroundUploadResponseHandlerObserver observer;

        /// <summary>
        /// Set to call the observer after the ServerResponseReader finishes.
        /// </summary>
        private Action result;

        #endregion

        #region Constructors

        public BackgroundUploadResponseHandler(
            Uri downloadLocation, 
            IBackgroundUploadResponseHandlerObserver observer)
        {
            Debug.Assert(downloadLocation != null);
            Debug.Assert(observer != null);

            this.downloadLocation = downloadLocation;
            this.observer = observer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle the response asynchronously.
        /// </summary>
        public void ReadJsonResponseFromDownloadLocation()
        {
            var asyncIsolatedStorageOperation = new BackgroundWorker<Action>(this);
            asyncIsolatedStorageOperation.Execute();
        }

        /// <summary>
        /// Reads, parses, and deltes the upload response from downloadLocation and then
        /// returns an action that will call the appropriate method on the IBackgroundUploadResponseHandlerObserver.
        /// </summary>
        /// <returns>
        /// An action that will call the approriate method on the IBackgroundUploadResponseHandlerObserver.
        /// This return value will be given as an argument to OnSuccess on the main/ui thread.
        /// </returns>
        public Action OnDoWork()
        {
            string response = this.ReadAndDeleteFromDownloadLocation();
            ServerResponseReader responseReader = ServerResponseReader.Instance;
            responseReader.Read(response, this);

            return this.result;
        }

        /// <summary>
        /// Called by the BackgroundWorker on the main/ui thread and given the result from OnDoWork.
        /// </summary>
        /// <param name="result">The result from OnDoWork</param>
        public void OnSuccess(Action result)
        {
            result();
        }

        /// <summary>
        /// Called by the BackgroundWorker on the main/ui thread if there was an exception during
        /// OnDoWork.
        /// </summary>
        /// <param name="exception">Exception thrown from OnDoWork.</param>
        public void OnError(Exception exception)
        {
            this.observer.OnError(exception);
        }

        /// <summary>
        /// Called when the parsed response is a successful json response.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="rawResult"></param>
        public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
        {
            this.result = () => this.observer.OnSuccessResponse(result, rawResult);
        }

        /// <summary>
        /// Called when the parsed response is a json response with an error.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public void OnErrorResponse(string code, string message)
        {
            this.result = () => this.observer.OnErrorResponse(code, message);
        }

        /// <summary>
        /// Called when the response did not contain JSON.
        /// </summary>
        /// <param name="exception"></param>
        public void OnInvalidJsonResponse(FormatException exception)
        {
            this.result = () => this.observer.OnError(exception);
        }

        private string ReadAndDeleteFromDownloadLocation()
        {
            string response;
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream inStream =
                    store.OpenFile(this.downloadLocation.OriginalString, FileMode.Open))
                {
                    var streamReader = new StreamReader(inStream);
                    response = streamReader.ReadToEnd();
                }

                store.DeleteFile(downloadLocation.OriginalString);
            }

            return response;
        }

        #endregion
    }
}
