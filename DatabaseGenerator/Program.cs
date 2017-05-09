using lvl.Ontology;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.DatabaseGenerator
{
    public class Program
    {
        /// <summary>
        /// An application to generate databases from a given application assembly.
        /// 
        /// Synopsis:
        /// lvl.DatabaseGenerator (--connection-string 'connection-string'){1} (--assembly-path 'assembly-path'){1} (--post-generation-script-bin 'path'){0,1}  (--pre-generation-script-bin 'path'){0,1} (--migrate){0,1} (--dry-run){0,1}
        /// 
        /// Example:
        /// lvl.DatabaseGenerator --connection-string "helloworld" --migrate --assembly-path "here" --pre-generation-script-bin 'there'
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var argumentParser = new ArgumentParser();

            var domainOptions = new DomainOptions
            {
                ConnectionString = argumentParser.GetRequired<string>(args, "--connection-string")
            };

            var databaseGenerationOptions = new DatabaseGenerationOptions
            {
                AssemblyPath = argumentParser.GetRequired<string>(args, "--assembly-path"),
                DryRun = argumentParser.HasFlag(args, "--dry-run"),
                Migrate = argumentParser.HasFlag(args, "--migrate"),
                PostGenerationScriptBin = argumentParser.GetOptional<string>(args, "--post-generation-script-bin"),
                PreGenerationScriptBin = argumentParser.GetOptional<string>(args, "--pre-generation-script-bin")
            };

            var assemblyLoader = new AssemblyLoader();
            assemblyLoader.Load(databaseGenerationOptions.AssemblyPath);

            var services = new ServiceCollection()
                .AddDomains(domainOptions)
                .AddDatabaseGeneration(databaseGenerationOptions)
                .BuildServiceProvider();

            var databaseGenerationRunner = services.GetRequiredService<DatabaseGenerationRunner>();
            databaseGenerationRunner.Run();
        }
    }
}
