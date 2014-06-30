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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;

    internal static class Platform
    {
        private static readonly Color LightThemeBackground = Color.FromArgb(255, 255, 255, 255);

        public static DisplayType GetDisplayType()
        {
            return DisplayType.Windows_Phone;
        }

        /// <summary>
        /// Gets the ThemeType for WP7 using PhoneBackgroundBrush.
        /// </summary>
        public static ThemeType GetThemeType()
        {
            Debug.Assert(
                Deployment.Current.CheckAccess(),
                "GetThemeType must be called on the UI Thread because it accesses the Application's Resources.");

            var backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
            if (LightThemeBackground == backgroundBrush.Color)
            {
                return ThemeType.Light;
            }

            return ThemeType.Dark;
        }

        public static ResponseType GetResponseType()
        {
            return ResponseType.Code;
        }

        public static string GetCurrentUICultureString()
        {
            return CultureInfo.CurrentUICulture.ToString();
        }

        public static void RegisterForCancel(object state, Action callback)
        {
            // No-op
        }

        public static void ReportProgress(object handler, LiveOperationProgress progress)
        {
            var progressHandler = handler as Action<LiveOperationProgress>;
            if (progressHandler != null)
            {
                progressHandler(progress);
            }
        }

        public static void SetDownloadRequestOption(HttpWebRequest request)
        {
            // Disable data buffering.  The Response will be available as soon as there is some data to be read.
            request.AllowReadStreamBuffering = false;
        }

        public static void StreamReadAsync(
            Stream stream, 
            byte[] buffer, 
            int offset, 
            int count, 
            SynchronizationContextWrapper syncContext, 
            Action<int, Exception> callback)
        {
            Debug.Assert(syncContext != null);

            stream.BeginRead(
                buffer,
                offset,
                count,
                delegate(IAsyncResult ar)
                {
                    if (ar.IsCompleted)
                    {
                        int bytesRead = 0;
                        Exception error = null;

                        try
                        {
                            bytesRead = stream.EndRead(ar);
                        }
                        catch (IOException ioe)
                        {
                            error = ioe;
                        }
                        finally
                        {
                            syncContext.Post(() => callback(bytesRead, error));
                        }
                    }
                },
                null);
        }

        public static void StreamWriteAsync(
            Stream stream, 
            byte[] buffer, 
            int offset, 
            int count, 
            SynchronizationContextWrapper syncContext, 
            Action<int, Exception> callback)
        {
            Debug.Assert(syncContext != null);

            stream.BeginWrite(
                buffer,
                offset,
                count,
                delegate(IAsyncResult ar)
                {
                    if (ar.IsCompleted)
                    {
                        Exception error = null;
                        try
                        {
                            stream.EndWrite(ar);
                            stream.Flush();
                        }
                        catch (IOException ioe)
                        {
                            error = ioe;
                        }
                        finally
                        {
                            syncContext.Post(() => callback(count, error));
                        }
                    }
                },
                null);
        }

        public static string GetLibraryHeaderValue()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string[] versions = assemblyName.Split(',')[1].Split('=')[1].Split('.');

            return string.Format(
                CultureInfo.InvariantCulture,
                "WindowsPhone/{0}.{1}_{2}.{3}",
                Environment.OSVersion.Version.Major,
                Environment.OSVersion.Version.Minor,
                versions[0],
                versions[1]);
        }

    }
}