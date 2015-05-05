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
    using System.Reflection;
    using System.Resources;

    internal static class ResourceHelper
    {
        private static readonly Dictionary<String, String> ErrorMappings;

        private static readonly ResourceManager PrimaryResourceManager;
#if !WINDOWS_PHONE
        private static readonly Lazy<ResourceManager> FallbackResourceManager;
#endif

        static ResourceHelper()
        {
            PrimaryResourceManager = new ResourceManager(typeof(Resources));
#if !WINDOWS_PHONE
            FallbackResourceManager = new Lazy<ResourceManager>(() =>
                new ResourceManager("Microsoft.Live.Internal.Resources", Assembly.GetAssembly(typeof(Resources))));
#endif
            ErrorMappings = new Dictionary<string, string>();
            ErrorMappings["AsyncOperationInProgress"] = "Another async operation is already in progress.";
            ErrorMappings["BackgroundTransferServiceAddError"] = "An error occurred while adding the request to the BackgroundTransferService.";
            ErrorMappings["BackgroundTransferServiceRemoveError"] = "An error occurred while removing a request to the BackgroundTransferService.";
            ErrorMappings["BackgroundUploadResponseHandlerError"] = "An error occurred while reading the response of the BackgroundUpload.";
            ErrorMappings["ConnectionError"] = "A connection to the server could not be established.";
            ErrorMappings["ConsentNotGranted"] = "User has not granted the application consent to access data in Windows Live.";
            ErrorMappings["FileNameInvalid"] = "Input parameter '{0}' is invalid.  '{0}' contains invalid characters.";
            ErrorMappings["InvalidNullOrEmptyParameter"] = "Input parameter '{0}' is invalid.  '{0}' cannot be null or empty.";
            ErrorMappings["InvalidNullParameter"] = "Input parameter '{0}' is invalid.  '{0}' cannot be null.";
            ErrorMappings["LoginPopupAlreadyOpen"] = "The login page is already open.";
            ErrorMappings["NoResponseData"] = "There was no response from the server for this request.";
            ErrorMappings["NotOnUiThread"] = "The method '{0}' must be called from the application's UI thread.";
            ErrorMappings["NoUploadLinkFound"] = "Could not determine the upload location.  Make sure the path points to a file resource id.";
            ErrorMappings["RelativeUrlRequired"] = "Input parameter '{0}' is invalid.  '{0}' must be a relative url.";
            ErrorMappings["RootVisualNotRendered"] = "Can not invoke the login page before the application root visual is rendered.";
            ErrorMappings["ServerError"] = "An error occurred while performing the operation. Please try again later.";
            ErrorMappings["ServerErrorWithStatus"] = "An error occurred while performing the operation. Server returned a response with status {0}.";
            ErrorMappings["StreamNotReadable"] = "Input parameter '{0}' is invalid.  Stream is not readable.";
            ErrorMappings["StreamNotWritable"] = "Input parameter '{0}' is invalid.  Stream is not writable.";
            ErrorMappings["UriMissingFileName"] = "Input parameter '{0}' is invalid. '{0}' must contain a filename.";
            ErrorMappings["UriMustBeRootedInSharedTransfers"] = "Input parameter '{0}' is invalid. '{0}' must be rooted in \\shared\\transfers.";
            ErrorMappings["UrlInvalid"] = "Input parameter '{0}' is invalid.  '{0}' must be a valid URI.";
            ErrorMappings["UserNotLoggedIn"] = "User did not log in.  Call LiveAuthClient.LoginAsync to log in.";
        }

        public static string GetString(string name)
        {
            if (ErrorMappings[name] != null)
            {
                return ErrorMappings[name];
            }

            try
            {
                return PrimaryResourceManager.GetString(name);
            }
            catch (MissingManifestResourceException)
            {
#if WINDOWS_PHONE
                throw;
#endif
            }

#if !WINDOWS_PHONE
            return FallbackResourceManager.Value.GetString(name);
#endif
        }
    }
}
