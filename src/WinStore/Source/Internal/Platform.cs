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
