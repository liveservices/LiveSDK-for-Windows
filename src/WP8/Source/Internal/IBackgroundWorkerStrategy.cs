namespace Microsoft.Live
{
    using System;

    internal interface IBackgroundWorkerStrategy<TResult>
    {
        TResult OnDoWork();
        void OnSuccess(TResult result);
        void OnError(Exception exception);
    }
}
