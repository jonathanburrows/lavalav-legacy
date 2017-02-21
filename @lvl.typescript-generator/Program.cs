using System;

namespace lvl.TypescriptGenerator
{
    public class Program
    {
        /// <summary>
        /// An application to generate typescript models from a given assembly.
        /// 
        /// Synopsis:
        /// @lvl.typescript-generator --assembly-path='assembly-path' --output-directory='output-directory' ['C# namespace'='npm package'[, 'C# namespace'='npm package'...]] 
        /// 
        /// Example:
        /// @lvl.typescript-generator --assembly-path=lvl.TestDomain.dll --output-directory='src/models' lvl.Ontology='@lvl/core'
        /// 
        /// </summary>
        /// <param name="args">The command line arguments used to generate the models.</param>
        public static void Main(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
