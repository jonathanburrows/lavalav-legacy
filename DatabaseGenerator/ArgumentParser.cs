using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    /// Provides ability to convert a set of command line arguments into options for database generation.
    /// </summary>
    public class ArgumentParser
    {
        private const string
            ConnectionStringKey = "--connection-string",
            AssemblyPathKey = "--assembly-path",
            PostGenerationScriptKey = "--post-generation-script-bin",
            PreGenerationScriptKey = "--pre-generation-script-bin",
            MigrateKey = "--migrate",
            DryRunKey = "--dry-run";

        /// <summary>
        /// Converts a set of command line arguments into options for generating a database.
        /// 
        /// Synopsis:
        /// (--connection-string 'connection-string'){1} (--assembly-path 'assembly-path'){1} (--post-generation-script-bin 'path'){0,1}  (--pre-generation-script-bin 'path'){0,1} (--migrate){0,1} (--dry-run){0,1}
        /// 
        /// Example:
        /// --connection-string "helloworld" --migrate --assembly-path "here" --pre-generation-script-bin 'there'
        /// 
        /// </summary>
        /// <param name="args">The set of command line arguments to be converted.</param>
        /// <returns>The converted set of command line arguments</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> has a null value.</exception> 
        /// <exception cref="ArgumentException">A required argument was missing from the arguments</exception>
        /// <exception cref="ArgumentException">A required argument flag was provided with no value</exception>
        /// <exception cref="ArgumentException">An optional argument flag was provided with no value</exception>
        public DatabaseGenerationOptions Parse(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var requiredParameters = new HashSet<string>(new[] { ConnectionStringKey, AssemblyPathKey });
            var booleanParameters = new HashSet<string>(new[] { MigrateKey, DryRunKey });
            var optionalParameters = new HashSet<string>(new[] { PreGenerationScriptKey, PostGenerationScriptKey });
            var allParameters = new HashSet<string>(requiredParameters.Union(booleanParameters).Union(optionalParameters));

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            foreach (var requiredParameter in requiredParameters)
            {
                if (!args.Any(arg => arg.Equals(requiredParameter, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ArgumentException($"Missing the required {requiredParameter} parameter");
                }
            }

            foreach (var requiredParameter in requiredParameters)
            {
                var i = IndexOf(args, requiredParameter);

                if (i < 0 || i == args.Length - 1 || allParameters.Contains(args[i + 1].ToLower()))
                {
                    throw new ArgumentException($"A value wasnt given for {requiredParameter}");
                }
            }

            foreach (var optionalParameter in optionalParameters)
            {
                var i = IndexOf(args, optionalParameter);
                if (i < 0)
                {
                    continue;
                }

                if (i == args.Length - 1 || allParameters.Contains(args[i + 1].ToLower()))
                {
                    throw new ArgumentException($"{optionalParameter} was declared but not given a value");
                }
            }

            var generationOptions = new DatabaseGenerationOptions();

            var connectionStringIndex = IndexOf(args, ConnectionStringKey);
            generationOptions.ConnectionString = args[connectionStringIndex + 1];

            var assemblyPathIndex = IndexOf(args, AssemblyPathKey);
            generationOptions.AssemblyPath = args[assemblyPathIndex + 1];

            var preGenerationScriptIndex = IndexOf(args, PreGenerationScriptKey);
            if (preGenerationScriptIndex >= 0)
            {
                generationOptions.PreGenerationScriptBin = args[preGenerationScriptIndex + 1];
            }

            var postGenerationScriptIndex = IndexOf(args, PostGenerationScriptKey);
            if (postGenerationScriptIndex >= 0)
            {
                generationOptions.PostGenerationScriptBin = args[postGenerationScriptIndex + 1];
            }

            generationOptions.Migrate = IndexOf(args, MigrateKey) > -1;
            generationOptions.DryRun = IndexOf(args, DryRunKey) > -1;

            return generationOptions;
        }

        private int IndexOf(string[] args, string key)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
