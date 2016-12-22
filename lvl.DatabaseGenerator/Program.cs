using lvl.Ontology;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;

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
            var options = argumentParser.Parse(args);

            var assemblyLoader = new AssemblyLoader();
            assemblyLoader.Load(options.AssemblyPath);

            var services = new ServiceCollection()
                .AddDomains(options.ConnectionString)
                .AddDatabaseGeneration()
                .BuildServiceProvider();
            var scriptRunner = services.GetRequiredService<ScriptRunner>();
            var configuration = services.GetRequiredService<Configuration>();

            using (var sessionFactory = configuration.BuildSessionFactory())
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                if (options.PreGenerationScriptBin != null)
                {
                    scriptRunner.RunScriptsInDirectory(options.PreGenerationScriptBin);
                }

                if (options.Migrate)
                {
                    var databaseMigrator = services.GetRequiredService<DatabaseMigrator>();

                    if (options.DryRun)
                    {
                        databaseMigrator.DryRun();
                    }
                    else
                    {
                        databaseMigrator.Migrate();
                    }
                }
                else
                {
                    var databaseCreator = services.GetRequiredService<DatabaseCreator>();

                    if (options.DryRun)
                    {
                        databaseCreator.DryRun();
                    }
                    else
                    {
                        databaseCreator.Create();
                    }
                }

                if (options.PostGenerationScriptBin != null)
                {
                    scriptRunner.RunScriptsInDirectory(options.PostGenerationScriptBin);
                }

                transaction.Commit();
            }
        }
    }
}
