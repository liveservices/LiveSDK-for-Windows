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
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The Platform class implements platform dependent methods
    /// </summary>
    internal static class Platform
    {
        public static string GetLibraryHeaderValue()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string[] versions = assemblyName.Split(',')[1].Split('=')[1].Split('.');

            return string.Format(
                CultureInfo.InvariantCulture,
                "Windows/.Net4_{0}.{1}Server",
                versions[0],
                versions[1]);
        }

        public static void SetDownloadRequestOption(HttpWebRequest request)
        {
            // No-op
        }

        public static void RegisterForCancel(object state, Action callback)
        {
            if (state is CancellationToken)
            {
                CancellationToken ct = (CancellationToken)state;
                ct.Register(callback);
            }
        }

        public static void StreamReadAsync(
            Stream stream,
            byte[] buffer,
            int offset,
            int count,
            SynchronizationContextWrapper syncContext,
            Action<int, Exception> callback)
        {
            Exception error = null;
            int length = 0;
            try
            {
                stream.BeginRead(buffer, offset, count,
                    delegate(IAsyncResult ar)
                    {
                        try
                        {
                            length = stream.EndRead(ar);
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }

                        callback(length, error);
                    },
                    null);
            }
            catch (Exception e)
            {
                error = e;
                callback(length, error);
            }
        }

        public static void StreamWriteAsync(
            Stream stream,
            byte[] buffer,
            int offset,
            int count,
            SynchronizationContextWrapper syncContext,
            Action<int, Exception> callback)
        {
            Exception error = null;
            int length = 0;

            try
            {
                stream.BeginWrite(buffer, offset, count,
                    delegate(IAsyncResult ar)
                    {
                        try
                        {
                            stream.EndWrite(ar);
                            stream.Flush();
                            length = count;
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }

                        callback(length, error);
                    },
                    null);
            }
            catch (Exception e)
            {
                error = e;
                callback(length, error);
            }
        } 

        public static void ReportProgress(object handler, LiveOperationProgress progress)
        {
            var progressHandler = handler as IProgress<LiveOperationProgress>;
            if (progressHandler != null)
            {
                progressHandler.Report(progress);
            }
        }
    }
}
