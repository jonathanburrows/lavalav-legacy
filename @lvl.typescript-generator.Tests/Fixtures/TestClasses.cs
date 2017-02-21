/// <summary>
/// Due to the large amount of classes, they were centralized into one file here.
/// </summary>
namespace lvl.TypeScriptGenerator.Tests.Fixtures
{
    using lvl.ExternalLibrary;

    internal class BareClass {
        public ThirdPartyClass Child { get; set; }
    }
}

namespace lvl.ExternalLibrary {
    internal class ThirdPartyClass { }
}