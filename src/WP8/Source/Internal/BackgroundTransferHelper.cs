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
