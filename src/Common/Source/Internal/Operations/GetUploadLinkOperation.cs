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

namespace Microsoft.Live.Operations
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// GetUploadLinkOperation will preform a GET to get the upload_location for an upload request.
    /// GetUploadLinkOperation will also add the original query string parameters it was given to the request
    /// and an additionaly overwrite parameter if it was NOT a file upload.
    /// </summary>
    internal class GetUploadLinkOperation : ApiOperation
    {
        #region Class member variables

        private const string UploadLocationKey = "upload_location";
        private const string FilePathPrefix = "/file.";

        #endregion

        #region Constructors

        public GetUploadLinkOperation(
            LiveConnectClient client, 
            Uri url, 
            string fileName, 
            OverwriteOption option, 
            SynchronizationContextWrapper syncContext)
            : base(client, url, ApiMethod.Get, null, syncContext)
        {
            this.FileName = fileName;
            this.OverwriteOption = option;
        }

        #endregion

        #region Properties

        public string FileName { get; private set; }

        public OverwriteOption OverwriteOption { get; private set; }

        private bool IsFilePath
        {
            get
            {
                string path = this.Url.AbsoluteUri.Substring(this.LiveClient.ApiEndpoint.Length).ToLowerInvariant();
                return path.StartsWith(FilePathPrefix) || path.StartsWith(FilePathPrefix.Substring(1));
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Uses the Task&lt;T&gt; pattern to return a LiveOperationResult.
        /// NOTE: This method will overwrite and uses its own OperationCompletedCallback.
        /// </summary>
        public Task<LiveOperationResult> ExecuteAsync()
        {
            var tcs = new TaskCompletionSource<LiveOperationResult>();

            this.OperationCompletedCallback += (LiveOperationResult result) =>
            {
                if (result.IsCancelled)
                {
                    tcs.TrySetCanceled();
                }
                else if (result.Error != null)
                {
                    tcs.TrySetException(result.Error);
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            };

            this.Execute();

            return tcs.Task;
        }

        #endregion

        #region Protected methods

        protected override void OnWebResponseReceived(WebResponse response)
        {
            LiveOperationResult opResult = this.CreateOperationResultFrom(response);

            if (opResult.Error != null)
            {
                this.OnOperationCompleted(opResult);
                return;
            }

            string uploadLink = null;
            if (opResult.Result != null && opResult.Result.ContainsKey(UploadLocationKey))
            {
                uploadLink = opResult.Result[UploadLocationKey] as string;
            }

            if (string.IsNullOrEmpty(uploadLink))
            {
                var error = new LiveConnectException(ApiOperation.ApiClientErrorCode,
                                                     ResourceHelper.GetString("NoUploadLinkFound"));
                opResult = new LiveOperationResult(error, false);
            }
            else
            {
                try
                {
                    Uri resourceUploadUrl = this.ConstructUploadUri(uploadLink);

                    opResult = new LiveOperationResult(null, resourceUploadUrl.OriginalString);

                }
                catch (LiveConnectException exp)
                {
                    opResult = new LiveOperationResult(exp, false);
                }
            }

            this.OnOperationCompleted(opResult);
        }

        #endregion

        #region Private methods

        private Uri ConstructUploadUri(string uploadLocation)
        {
            Uri resourceUrl;
            try
            {
                // Do not forget to append the query string that was originally sent.
                // this.Url contains the original query string plus suppress_response_codes and suppress_redirects.
                // We will just reuse that.
                string originalQueryString = this.Url.Query;

                if (!string.IsNullOrWhiteSpace(originalQueryString))
                {
                    // Uri.Query return contains "?" (e.g., ?key=value)
                    int indexOfQuestion = originalQueryString.IndexOf('?');
                    if (indexOfQuestion != -1)
                    {
                        originalQueryString = originalQueryString.Substring(indexOfQuestion + 1);
                    }

                    uploadLocation = uploadLocation.Contains("?")
                                         ? uploadLocation + "&" + originalQueryString
                                         : uploadLocation + "?" + originalQueryString;
                }

                resourceUrl = new Uri(uploadLocation, UriKind.Absolute);
            }
            catch (FormatException exp)
            {
                throw new LiveConnectException(
                    ApiOperation.ApiClientErrorCode,
                    ResourceHelper.GetString("NoUploadLinkFound"),
                    exp);
            }

            string query = resourceUrl.Query;
            string url = resourceUrl.GetComponents(
                UriComponents.SchemeAndServer | UriComponents.Path | UriComponents.KeepDelimiter, 
                UriFormat.SafeUnescaped);

            var sb = new StringBuilder(url);
            if (!this.IsFilePath)
            {
                sb.AppendUrlPath(Uri.EscapeDataString(this.FileName));
            }

            bool hasQuery = !string.IsNullOrEmpty(query);
            if (hasQuery)
            {
                sb.Append(query);
            }

            // Only add the overwrite parameter for folder uploads,
            // since overwrite does not make sense on file path uploads (because they are always overwriting).
            if (!this.IsFilePath)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.AppendQueryParam(QueryParameters.Overwrite, QueryParameters.GetOverwriteValue(this.OverwriteOption));
            }

            try
            {
                return new Uri(sb.ToString(), UriKind.Absolute);
            }
            catch (FormatException exp)
            {
                throw new LiveConnectException(
                    ApiOperation.ApiClientErrorCode,
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("FileNameInvalid"), "fileName"),
                    exp);
            }
        }

        #endregion
    }
}
