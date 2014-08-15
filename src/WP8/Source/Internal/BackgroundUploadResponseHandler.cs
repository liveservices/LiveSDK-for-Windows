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
