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
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Live.Operations;

    /// <summary>
    /// This class contain the public methods that LiveConnectClient implements.
    /// The methods in this partial class implement the Task async pattern.
    /// </summary>
    public partial class LiveConnectClient
    {
        #region Public Methods
        /// <summary>
        /// Makes a GET call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> GetAsync(string path)
        {
            return this.Api(path, ApiMethod.Get, null, new CancellationToken(false));
        }

        /// <summary>
        /// Makes a GET call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource.</param>
        /// <param name="ct">a token that is used to cancel the get operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> GetAsync(string path, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Get, null, ct);
        }

        /// <summary>
        /// Makes a DELETE call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource being deleted.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> DeleteAsync(string path)
        {
            return this.Api(path, ApiMethod.Delete, null, new CancellationToken(false));
        }

        /// <summary>
        /// Makes a DELETE call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource being deleted.</param>
        /// <param name="ct">a token that is used to cancel the delete operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> DeleteAsync(string path, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Delete, null, ct);
        }

        /// <summary>
        /// Makes a POST call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource collection to which the new object should be added.</param>
        /// <param name="body">properties of the new resource in json.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PostAsync(string path, string body)
        {
            return this.Api(path, ApiMethod.Post, body, new CancellationToken(false));
        }

        /// <summary>
        /// Makes a POST call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource collection to which the new object should be added.</param>
        /// <param name="body">properties of the new resource in name-value pairs.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PostAsync(string path, IDictionary<string, object> body)
        {
            return this.Api(path, ApiMethod.Post, ApiOperation.SerializePostBody(body), new CancellationToken(false));
        }

        /// <summary>
        /// Makes a POST call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource collection to which the new object should be added.</param>
        /// <param name="body">properties of the new resource in json.</param>
        /// <param name="ct">a token that is used to cancel the post operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PostAsync(string path, string body, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Post, body, ct);
        }

        /// <summary>
        /// Makes a POST call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource collection to which the new object should be added.</param>
        /// <param name="body">properties of the new resource in name-value pairs.</param>
        /// <param name="ct">a token that is used to cancel the post operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PostAsync(string path, IDictionary<string, object> body, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Post, ApiOperation.SerializePostBody(body), ct);
        }

        /// <summary>
        /// Makes a PUT call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource to be updated.</param>
        /// <param name="body">properties of the updated resource in json.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PutAsync(string path, string body)
        {
            return this.Api(path, ApiMethod.Put, body, new CancellationToken(false));
        }

        /// <summary>
        /// Makes a PUT call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource to be updated.</param>
        /// <param name="body">properties of the updated resource in name-value pairs.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PutAsync(string path, IDictionary<string, object> body)
        {
            return this.Api(path, ApiMethod.Put, ApiOperation.SerializePostBody(body), new CancellationToken(false));
        }

        /// <summary>
        /// Makes a PUT call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource to be updated.</param>
        /// <param name="body">properties of the updated resource in json.</param>
        /// <param name="ct">a token that is used to cancel the put operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PutAsync(string path, string body, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Put, body, ct);
        }

        /// <summary>
        /// Makes a PUT call to Api service
        /// </summary>
        /// <param name="path">relative path to the resource to be updated.</param>
        /// <param name="body">properties of the updated resource in json.</param>
        /// <param name="ct">a token that is used to cancel the put operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> PutAsync(string path, IDictionary<string, object> body, CancellationToken ct)
        {
            return this.Api(path, ApiMethod.Put, ApiOperation.SerializePostBody(body), ct);
        }

        /// <summary>
        /// Move a file from one location to another
        /// </summary>
        /// <param name="path">relative path to the file resource to be moved.</param>
        /// <param name="destination">relative path to the folder resource where the file should be moved to.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> MoveAsync(string path, string destination)
        {
            return this.MoveOrCopy(path, destination, ApiMethod.Move, new CancellationToken(false));
        }

        /// <summary>
        /// Move a file from one location to another
        /// </summary>
        /// <param name="path">relative path to the file resource to be moved.</param>
        /// <param name="destination">relative path to the folder resource where the file should be moved to.</param>
        /// <param name="ct">a token that is used to cancel the move operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> MoveAsync(string path, string destination, CancellationToken ct)
        {
            return this.MoveOrCopy(path, destination, ApiMethod.Move, ct);
        }

        /// <summary>
        ///  Copy a file to another location.
        /// </summary>
        /// <param name="path">relative path to the file resource to be copied.</param>
        /// <param name="destination">relative path to the folder resource where the file should be copied to.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> CopyAsync(string path, string destination)
        {
            return this.MoveOrCopy(path, destination, ApiMethod.Copy, new CancellationToken(false));
        }

        /// <summary>
        /// Copy a file to another location.
        /// </summary>
        /// <param name="path">relative path to the file resource to be copied.</param>
        /// <param name="destination">relative path to the folder resource where the file should be copied to.</param>
        /// <param name="ct">a token that is used to cancel the copy operation.</param>
        /// <returns>A Task object representing the asynchronous operation.</returns>
        public Task<LiveOperationResult> CopyAsync(string path, string destination, CancellationToken ct)
        {
            return this.MoveOrCopy(path, destination, ApiMethod.Copy, ct);
        }

        #endregion

        #region Private methods

        private Task<LiveOperationResult> MoveOrCopy(string path, string destination, ApiMethod method, CancellationToken ct)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentException("destination");
            }

            string body = string.Format(CultureInfo.InvariantCulture, ApiOperation.MoveRequestBodyTemplate, destination);

            return this.Api(path, method, body, ct);
        }

        private Task<LiveOperationResult> Api(string path, ApiMethod method, string body, CancellationToken ct)
        {
            ApiOperation op = this.GetApiOperation(path, method, body);
            return this.ExecuteApiOperation(op, ct);
        }

        private Task<LiveOperationResult> ExecuteApiOperation(ApiOperation op, CancellationToken ct)
        {
            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            var tcs = new TaskCompletionSource<LiveOperationResult>();
            op.OperationCompletedCallback = (LiveOperationResult opResult) =>
            {
                if (opResult.IsCancelled)
                {
                    tcs.TrySetCanceled();
                }
                else if (opResult.Error != null)
                {
                    tcs.TrySetException(opResult.Error);
                }
                else
                {
                    tcs.TrySetResult(opResult);
                }
            };

            ct.Register(op.Cancel);

            op.Execute();

            return tcs.Task;
        }

        #endregion
    }
}
