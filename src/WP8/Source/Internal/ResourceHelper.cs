namespace Microsoft.Live
{
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
}
