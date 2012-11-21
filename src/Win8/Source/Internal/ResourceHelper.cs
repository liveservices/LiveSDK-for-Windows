namespace Microsoft.Live
{
    using System;

    using Windows.ApplicationModel.Resources.Core;

    internal static class ResourceHelper
    {
        public static string GetString(string name)
        {
            return ResourceManager.Current.MainResourceMap.GetValue("ms-resource:///Microsoft.Live/Resources/" + name).ValueAsString;
        }
    }
}
