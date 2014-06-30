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
