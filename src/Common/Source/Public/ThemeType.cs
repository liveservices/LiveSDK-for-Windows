namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// Enum for the theme paramter values.
    /// </summary>
#if NETFX_CORE
    public enum ThemeType
#elif WINDOWS_PHONE || WEB || DESKTOP
    internal enum ThemeType
#else
#error Must implement this for this platform type!
#endif
    {
        Dark,
        Light,
        Win7,
        Win8,
        Default,
        None
    }
}
