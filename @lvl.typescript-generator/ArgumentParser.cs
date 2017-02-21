using System;
using System.IO;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Provides ability to convert a set of command line arguments into options for typescript generation.
    /// </summary>
    public class ArgumentParser
    {
        private const string AssemblyPathKey = "--assembly-path";
        private const string OutputDirectoryKey = "--output-directory";

        /// <summary>
        /// Converts a set of command line arguments into options for generating typescript models.
        /// 
        /// Synopsis:
        /// --assembly-path='assembly-path' --output-directory='output-directory' ['C# namespace'='npm path'[, 'C# namespacespace'='npm path'...]]
        /// 
        /// </summary>
        /// <param name="args">The set of command line arguments to be converted.</param>
        /// <returns>The options for generating the typescript.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentException">Assembly path was not specified.</exception>
        /// <exception cref="FileNotFoundException">The given assembly could not be found.</exception>
        /// <exception cref="ArgumentException">The output directory was not specified.</exception>
        public GenerationOptions Parse(string[] args)
        {
            throw new ArgumentNullException();
        }
    }
}
