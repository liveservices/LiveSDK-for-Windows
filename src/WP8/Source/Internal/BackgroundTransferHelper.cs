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
    using System.Text.RegularExpressions;

    using Microsoft.Phone.BackgroundTransfer;

    /// <summary>
    /// Contains various helper methods for working with BackgroundTransferRequests.
    /// </summary>
    internal static class BackgroundTransferHelper
    {
        #region Constants and Enums

        /// <summary>
        /// Prefix of BackgroundTransferRequest's Tag to denote the request was created by us.
        /// </summary>
        public const string Tag = "LiveSDK";

        /// <summary>
        /// The regex to check if the given uri is in /shared/transfers
        /// </summary>
        private static readonly Regex SharedTransfersPathRegeEx;

        #endregion

        #region Constructors

        static BackgroundTransferHelper()
        {
            SharedTransfersPathRegeEx = 
                new Regex(@"^[\\/]?shared[\\/]transfers[\\/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the given BackgroundTransferRequest was created by the Live SDK
        /// </summary>
        /// <param name="request">the request to check</param>
        /// <returns>true if the given BTR belongs to the Live SDK</returns>
        public static bool BelongsToLiveSdk(BackgroundTransferRequest request)
        {
            return !string.IsNullOrEmpty(request.Tag) && request.Tag.StartsWith(Tag, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the TransferPreferences enum that cooresponds to the given BackgroundTransferPreferences enum.
        /// </summary>
        public static TransferPreferences GetTransferPreferences(
            BackgroundTransferPreferences backgroundTransferPreferences)
        {
            switch (backgroundTransferPreferences)
            {
                case BackgroundTransferPreferences.None:
                    return TransferPreferences.None;
                case BackgroundTransferPreferences.AllowBattery:
                    return TransferPreferences.AllowBattery;
                case BackgroundTransferPreferences.AllowCellular:
                    return TransferPreferences.AllowCellular;
                case BackgroundTransferPreferences.AllowCellularAndBattery:
                    return TransferPreferences.AllowCellularAndBattery;
                default:
                    throw new Exception("Must update method to handle new BackgroundTransferPreferences enum.");
            }
        }

        /// <summary>
        /// checks to see if the request has already been removed (i.e., canceled) from the BackgroundTransferService.
        /// </summary>
        /// <returns>true if the request has already been removed (i.e., canceled) from the BackgroundTransferService.</returns>
        public static bool IsCanceledRequest(BackgroundTransferRequest request)
        {
            return request.TransferError is InvalidOperationException &&
                   request.TransferError.Message == "The request has already been canceled";
        }

        /// <summary>
        /// Checks to see if the given Uri is rooted in /shared/transfers
        /// </summary>
        /// <param name="location">the uri to check</param>
        /// <returns>true if the given Uri is rooted in /shared/transfers</returns>
        public static bool IsRootedInSharedTransfers(Uri location)
        {
            return SharedTransfersPathRegeEx.IsMatch(location.OriginalString);
        }

        /// <summary>
        /// Checks if the given response code is a 2XX response.
        /// </summary>
        /// <param name="statusCode">response code to check</param>
        /// <returns>true if the given response code is 2XX</returns>
        public static bool IsSuccessfulStatusCode(long statusCode)
        {
            return (statusCode / 100) == 2;
        }

        /// <summary>
        /// Checks to see if the given BackgroundTransferRequest is a download request.
        /// </summary>
        /// <param name="request">request to check</param>
        /// <returns>true if the given request is a download</returns>
        public static bool IsDownloadRequest(BackgroundTransferRequest request)
        {
            return request.Method.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks to see if the given BackgroundTransferRequest is an upload request.
        /// </summary>
        /// <param name="request">request to check</param>
        /// <returns>true if the given request is an upload</returns>
        public static bool IsUploadRequest(BackgroundTransferRequest request)
        {
            return request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
