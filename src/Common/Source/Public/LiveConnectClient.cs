namespace Microsoft.Live
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    using Microsoft.Live.Operations;

    /// <summary>
    /// This is the class that applications use to interact with the Api service.
    /// </summary>
    public sealed partial class LiveConnectClient
    {
        #region Private Fields

        private const string DefaultApiEndpoint = "https://apis.live.net/v5.0";

        // syncContext is used in the other partial classes.
        private SynchronizationContextWrapper syncContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new LiveConnectClient instance.
        /// </summary>
        /// <param name="session">the session object that contains the authentication information.</param>
        public LiveConnectClient(LiveConnectSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.Session = session;
            this.syncContext = SynchronizationContextWrapper.Current;

#if DEBUG
            this.ApiEndpoint =
                string.IsNullOrEmpty(LiveConnectClient.ApiEndpointOverride)
                ? LiveConnectClient.DefaultApiEndpoint
                : LiveConnectClient.ApiEndpointOverride;
#else
            this.ApiEndpoint = LiveConnectClient.DefaultApiEndpoint;
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// The current session object.
        /// </summary>
        public LiveConnectSession Session { get; internal set; }

#if DEBUG
        /// <summary>
        /// Allows the application to override the default api endpoint.
        /// </summary>
        public static string ApiEndpointOverride { get; set; }
#endif

        /// <summary>
        /// The current api endpoint.
        /// </summary>
        internal string ApiEndpoint { get; set; }

        #endregion

        #region Private, Internal Methods

        internal Uri GetResourceUri(string path, ApiMethod method)
        {
            try
            {
                if ((path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                   !path.StartsWith(this.ApiEndpoint, StringComparison.OrdinalIgnoreCase)) ||
                    path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    return new Uri(path, UriKind.Absolute);
                }

                StringBuilder sb;
                if (path.StartsWith(this.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
                {
                    sb = new StringBuilder(path);
                }
                else
                {
                    sb = new StringBuilder(this.ApiEndpoint);
                    sb = sb.AppendUrlPath(path);
                }

                var resourceUrl = new Uri(sb.ToString(), UriKind.Absolute);
                sb.Append(string.IsNullOrEmpty(resourceUrl.Query) ? "?" : "&");

                if (method != ApiMethod.Download)
                {
                    sb.AppendQueryParam(QueryParameters.SuppressResponseCodes, "true");
                    sb.Append("&").AppendQueryParam(QueryParameters.SuppressRedirects, "true");
                }

                return new Uri(sb.ToString(), UriKind.Absolute);
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), "path"),
                    "path");
            }
        }

        private static bool IsAbsolutePath(string path)
        {
            return path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
        }

        private ApiOperation GetApiOperation(string path, ApiMethod method, string body)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("path");
            }

            if (IsAbsolutePath(path))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentUICulture, ResourceHelper.GetString("RelativeUrlRequired"), "path"),
                    "path");
            }

            Uri apiUri = this.GetResourceUri(path, method);

            if (this.Session == null)
            {
                throw new LiveConnectException(ApiOperation.ApiClientErrorCode, ResourceHelper.GetString("UserNotLoggedIn"));
            }

            ApiOperation operation = null;

            switch (method)
            {
                case ApiMethod.Get:
                case ApiMethod.Delete:
                    operation = new ApiOperation(this, apiUri, method, null, null);
                    break;

                case ApiMethod.Post:
                case ApiMethod.Put:
                case ApiMethod.Copy:
                case ApiMethod.Move:
                    if (body == null)
                    {
                        throw new ArgumentNullException("body");
                    }

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        throw new ArgumentException("body");
                    }

                    operation = new ApiWriteOperation(this, apiUri, method, body, null);
                    break;

                default:
                    Debug.Assert(false, "method not suppported.");
                    break;
            }

            return operation;
        }

        #endregion
    }
}
