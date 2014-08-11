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
    using System.Globalization;

    using Windows.ApplicationModel.Resources.Core;

    internal static class Platform
    {
        private const string OSVersion = "8";

        public static DisplayType GetDisplayType()
        {
            return DisplayType.WinDesktop;
        }

        public static ThemeType GetThemeType()
        {
            return ThemeType.Win8;
        }

        public static ResponseType GetResponseType()
        {
            return ResponseType.Token;
        }

        public static string GetCurrentUICultureString()
        {
            return ResourceContext.GetForCurrentView().Languages[0];
        }

        public static void ReportProgress(object handler, LiveOperationProgress progress)
        {
            var progressHandler = handler as IProgress<LiveOperationProgress>;
            if (progressHandler != null)
            {
                progressHandler.Report(progress);
            }
        }

        public static string GetLibraryHeaderValue()
        {
            string assemblyName = typeof(LiveAuthClient).AssemblyQualifiedName;
            string[] versions = assemblyName.Split(',')[2].Split('=')[1].Split('.');

            return string.Format(
                CultureInfo.InvariantCulture,
                "Windows/XAML{0}_{1}.{2}",
                Platform.OSVersion,
                versions[0],
                versions[1]);
        }
    }
}
