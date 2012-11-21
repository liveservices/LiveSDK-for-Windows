namespace Microsoft.Live.Controls
{
    using System.Resources;

    internal static class ResourceHelper
    {
        private static readonly ResourceManager stringResourceManager;
        private static readonly ResourceManager errorResourceManager;

        static ResourceHelper()
        {
            stringResourceManager = new ResourceManager(typeof(Microsoft.Live.Controls.Resources.WLText));
            errorResourceManager = new ResourceManager(typeof(Microsoft.Live.Controls.Resources.Errors));
        }

        public static string GetErrorString(string name)
        {
            return errorResourceManager.GetString(name);
        }

        public static string GetResourceString(string name)
        {
            return stringResourceManager.GetString(name);
        }
    }
}
