using System.Collections.Generic;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Configurations on how the typescript will be generated.
    /// </summary>
    public class GenerationOptions
    {
        /// <summary>The path to the assembly which contains the models to be generated.</summary>
        public string AssemblyPath { get; set; }

        /// <summary>The directory where the typescript will be generated to.</summary>
        public string OutputPath { get; set; }

        /// <summary>A set of mappings from C# namespaces to NPM Packages</summary>
        public IReadOnlyDictionary<string, string> PackageByNamespace { get; set; }
    }
}
