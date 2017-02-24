using System;

namespace lvl.TypescriptGenerator
{
    public class Program
    {
        /// <summary>
        /// An application to generate typescript models from a given assembly.
        /// 
        /// Synopsis:
        /// @lvl.typescript-generator --assembly-path='assembly-path' --output-bin='output-bin' [--decorator-path='decorator-path'] ['C# namespace'='npm package'[, 'C# namespace'='npm package'...]] 
        /// 
        /// Example:
        /// @lvl.typescript-generator --assembly-path=lvl.TestDomain.dll --output-bin='src/models' lvl.Ontology='@lvl/core'
        /// 
        /// </summary>
        /// <param name="args">The command line arguments used to generate the models.</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception> 
        public static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException();
            }
            throw new NotImplementedException();
        }
    }
}
