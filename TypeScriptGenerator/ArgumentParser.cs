using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Provides ability to convert a set of command line arguments into options for typescript generation.
    /// </summary>
    public class ArgumentParser
    {
        private const string AssemblyPathKey = "--assembly-path";
        private const string OutputBinKey = "--output-bin";
        private const string DecoratorPathKey = "--decorator-path";
        private const string DefaultDecoratorPath = "@lvl/front-end";

        /// <summary>
        /// Converts a set of command line arguments into options for generating typescript models.
        /// 
        /// Synopsis:
        /// --assembly-path='assembly-path' --output-bin='output-bin' [--decorator-path='decorator-path'] ['C# namespace'='npm path'[, 'C# namespacespace'='npm path'...]]
        /// 
        /// </summary>
        /// <param name="args">The set of command line arguments to be converted.</param>
        /// <returns>The options for generating the typescript.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentException">Assembly path was not specified.</exception>
        /// <exception cref="ArgumentException">The output bin was not specified.</exception>
        /// <exception cref="ArgumentException">Multiple assembly paths were specified.</exception>
        /// <exception cref="ArgumentException">Multiple output bins were specified.</exception>
        /// <exception cref="ArgumentException">Multiple decorator paths were specified.</exception>
        /// <exception cref="ArgumentException">The same C# Namespace was mapped twice.</exception>
        /// <exception cref="FileNotFoundException">The given assembly could not be found.</exception>
        public TypeScriptGenerationOptions Parse(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var assemblyPaths = args.Where(arg => arg.StartsWith($"{AssemblyPathKey}=", StringComparison.OrdinalIgnoreCase));
            // ReSharper disable PossibleMultipleEnumeration It is as it's supposed to be.
            if (!assemblyPaths.Any())
            {
                throw new ArgumentException($"No {AssemblyPathKey} was specified.");
            }

            if (assemblyPaths.Count() > 1)
            {
                throw new ArgumentException($"{assemblyPaths.Count()} {AssemblyPathKey}s were specified, when expecting 1.");
            }

            var assemblyPath = GetValue(assemblyPaths.Single());
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException($"The assembly could not be found at ${assemblyPath}");
            }

            var outputBins = args.Where(arg => arg.StartsWith($"{OutputBinKey}=", StringComparison.OrdinalIgnoreCase));
            if (!outputBins.Any())
            {
                throw new ArgumentException($"No {OutputBinKey} was specified.");
            }

            if (outputBins.Count() > 1)
            {
                throw new ArgumentException($"{outputBins.Count()} {OutputBinKey}s were specified, when expecting 1.");
            }

            var outputBin = GetValue(outputBins.Single());

            var decoratorPaths = args.Where(arg => arg.StartsWith($"{DecoratorPathKey}=", StringComparison.OrdinalIgnoreCase));
            if (decoratorPaths.Count() > 1)
            {
                throw new ArgumentException($"{decoratorPaths.Count()} {DecoratorPathKey}s were specified, when there's a maximum of 1.");
            }

            var decoratorPath = decoratorPaths.Any() ? GetValue(decoratorPaths.Single()) : DefaultDecoratorPath;

            var namespaceToPackages =
                from arg in args
                where !arg.StartsWith(AssemblyPathKey)
                where !arg.StartsWith(OutputBinKey)
                where !arg.StartsWith(DecoratorPathKey)
                select arg;

            var namespaces = namespaceToPackages.Select(arg => arg.Split('=').First());
            var namespaceConflicts = namespaces.GroupBy(x => x).Where(nc => nc.Count() > 1).Select(n => n.Key);
            if (namespaceConflicts.Any())
            {
                throw new ArgumentException($"The namespace(s) {string.Join(", ", namespaceConflicts)} were specified multiple times.");
            }

            var packagesForNamespace = namespaceToPackages.ToDictionary(GetKey, GetValue);

            return new TypeScriptGenerationOptions
            {
                AssemblyPath = assemblyPath,
                OutputBin = outputBin,
                DecoratorPath = decoratorPath,
                PackageForNamespace = packagesForNamespace
            };
        }

        private string GetKey(string keyValuePair) => keyValuePair.Split('=').First();

        private string GetValue(string keyValuePair)
        {
            var delimiter = '=';
            var rawValue = keyValuePair.Split(delimiter).Last();
            return rawValue.Replace("\"", string.Empty);
        }
    }
}
