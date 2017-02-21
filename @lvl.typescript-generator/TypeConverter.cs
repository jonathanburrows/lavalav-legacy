using System;
using System.Collections.Generic;
using System.Reflection;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Converts types from C# to TypeScript.
    /// </summary>
    public class TypeConverter
    {
        /// <summary>
        /// Constructs a typescript object which is equivilant the given C# type.
        /// </summary>
        /// <param name="converting">The type to be converted to typescript.</param>
        /// <param name="packageForAssembly">The list of paths to npm modules, based on assembly.</param>
        /// <returns>The content of the converted typescript type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converting"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageForAssembly"/> is null.</exception>
        public string CsToTypeScript(Type converting, IReadOnlyDictionary<Assembly, string> packageForAssembly)
        {
            throw new NotImplementedException();
        }
    }
}
