using System.Resources;
using System.Runtime.CompilerServices;

[assembly: NeutralResourcesLanguageAttribute("en")]

#if DEBUG
[assembly: InternalsVisibleTo("Microsoft.Live.Win8.UnitTests")]
[assembly: InternalsVisibleTo("TestModernNetLibrary")]
#endif
