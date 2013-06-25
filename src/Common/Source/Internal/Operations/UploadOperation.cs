namespace Microsoft.Live.Operations
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    internal class UploadOperation : ApiOperation
    {
        #region Class members variables

        private const long UnknownFileSize = 0;

        #endregion

        #region Instance member variables

        private readonly long totalBytesToSend;
        private Stream requestStream;
        private readonly object progress;

        #endregion

        #region Contructors

        public UploadOperation(
            LiveConnectClient client, 
            Uri url, 
            string fileName,
            Stream inputStream, 
            OverwriteOption option,
            object progress,
            SynchronizationContextWrapper syncContext)
            : base(client, url, ApiMethod.Upload, null, syncContext)
        {
            this.FileName = fileName;
            this.OverwriteOption = option;
            this.InputStream = inputStream;
            this.progress = progress;

            this.totalBytesToSend = this.InputStream.CanSeek ?
                                    this.InputStream.Length :
                                    UploadOperation.UnknownFileSize;
        }

        #endregion

        #region Properties

        public string FileName { get; private set; }

        public Stream InputStream { get; private set; }

        public OverwriteOption OverwriteOption { get; private set; }

        #endregion

        #region Methods

        protected override void OnExecute()
        {
            var getUploadLinkOp = new GetUploadLinkOperation(
                    this.LiveClient,
                    this.Url,
                    this.FileName,
                    this.OverwriteOption,
                    null);

            getUploadLinkOp.OperationCompletedCallback = this.OnGetUploadLinkCompleted;
            getUploadLinkOp.Execute();
        }

        private void OnGetUploadLinkCompleted(LiveOperationResult result)
        {
            if (result.Error != null || result.IsCancelled)
            {
                this.OnOperationCompleted(result);
                return;
            }

            this.Url = new Uri(result.RawResult, UriKind.Absolute);

            // NOTE: the GetUploadLinkOperation will return a uri with the overwite, suppress_response_codes,
            // and suppress_redirect query parameters set.
            Debug.Assert(this.Url.Query != null);
            Debug.Assert(this.Url.Query.Contains(QueryParameters.Overwrite));
            Debug.Assert(this.Url.Query.Contains(QueryParameters.SuppressRedirects));
            Debug.Assert(this.Url.Query.Contains(QueryParameters.SuppressResponseCodes));

            if (this.PrepareRequest())
            {
                try
                {
                    var httpRequest = this.Request as HttpWebRequest;
                    if (httpRequest != null && this.InputStream.CanSeek)
                    {
                        httpRequest.AllowWriteStreamBuffering = false;
                        httpRequest.ContentLength = this.InputStream.Length;
                    }

                    this.Request.BeginGetRequestStream(this.OnGetRequestStreamCompleted, null);
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        this.OnWebResponseReceived(exception.Response);
                    }
                }
            }
        }

        protected override void OnGetRequestStreamCompleted(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                try
                {
                    this.OnRequestStreamReady(this.Request.EndGetRequestStream(ar));
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        this.OnWebResponseReceived(exception.Response);
                    }
                }
                catch (IOException)
                {
                    this.OnWebResponseReceived(null);
                }
            }
        }

        private void OnRequestStreamReady(Stream requestStream)
        {
            this.requestStream = requestStream;
            Debug.Assert(this.requestStream != null);

            var copyOperation = new StreamCopyOperation(
                    this.LiveClient,
                    ApiMethod.Upload,
                    this.InputStream,
                    this.requestStream,
                    this.totalBytesToSend,
                    this.progress,
                    this.Dispatcher,
                    (isCancelled, error) =>
                    {
                        if (isCancelled)
                        {
                            this.Cancel();
                        }

                        this.OnCopyCompleted(error);
                    });

            Platform.RegisterForCancel(null, copyOperation.Cancel);

            copyOperation.Execute();
        }

        private void OnCopyCompleted(Exception error)
        {
            this.requestStream.Dispose();
   
            if (error != null)
            {
                this.OnOperationCompleted(new LiveOperationResult(error, false));
            }
            else
            {
                try
                {
                    this.Request.BeginGetResponse(this.OnGetResponseCompleted, null);
                }
                catch (WebException exception)
                {
                    if (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        this.OnCancel();
                    }
                    else
                    {
                        this.OnWebResponseReceived(exception.Response);
                    }
                }
            }
        }

        #endregion
    }
}
