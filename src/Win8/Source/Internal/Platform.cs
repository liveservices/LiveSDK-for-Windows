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
            return ResourceManager.Current.DefaultContext.Languages[0];
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
