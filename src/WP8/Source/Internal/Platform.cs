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