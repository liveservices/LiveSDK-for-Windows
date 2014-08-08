using System.Resources;
using System.Runtime.CompilerServices;
using System.Reflection;

[assembly: NeutralResourcesLanguageAttribute("en")]

#if DEBUG
[assembly: InternalsVisibleTo("Microsoft.Live.WP8.UnitTests")]
#endif
[assembly: AssemblyDescriptionAttribute("Live SDK")]
