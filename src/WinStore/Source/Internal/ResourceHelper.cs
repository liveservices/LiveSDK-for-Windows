namespace Microsoft.Live
{
    using System;

#if SILVERLIGHT
    using System.Resources;

    internal static class ResourceHelper
    {
        private static readonly ResourceManager resourceManager;

        static ResourceHelper()
        {
            resourceManager = new ResourceManager(typeof(Resources));
        }

        public static string GetString(string name)
        {
            return resourceManager.GetString(name);
        }
    }
#else

    using Windows.ApplicationModel.Resources.Core;

    internal static class ResourceHelper
    {
        public static string GetString(string name)
        {
            return ResourceManager.Current.MainResourceMap.GetValue("ms-resource:///Microsoft.Live/Resources/" + name).ValueAsString;
        }
    }
#endif
}
