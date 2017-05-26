using System.Collections.Generic;

// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Configurations on how the typescript will be generated.
    /// </summary>
    public class TypeScriptGenerationOptions
    {
        /// <summary>
        ///     The path to the assembly which contains the models to be generated.
        /// </summary>
        public string AssemblyPath { get; set; }

        /// <summary>
        ///     The directory where the typescript will be generated to.
        /// </summary>
        public string OutputBin { get; set; }

        /// <summary>
        ///     (optional) The directory which the decorator attributes may be found in.
        /// </summary>
        public string DecoratorPath { get; set; }

        /// <summary>
        ///     A set of mappings from C# namespaces to NPM Packages.
        /// </summary>
        public IReadOnlyDictionary<string, string> PackageForNamespace { get; set; }
    }
}
