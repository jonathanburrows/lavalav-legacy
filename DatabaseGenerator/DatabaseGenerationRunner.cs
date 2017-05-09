using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    ///     This will contain all the logic to generate databases based on the registered options.
    /// </summary>
    /// <remarks>
    ///     This was done since much of the same logic was being repeating across projects.
    /// </remarks>
    public class DatabaseGenerationRunner
    {
        private ScriptRunner ScriptRunner { get; }
        private Configuration Configuration { get; }
        private DatabaseMigrator DatabaseMigrator { get; }
        private DatabaseCreator DatabaseCreator { get; }
        private DatabaseGenerationOptions Options { get; }

        public DatabaseGenerationRunner(ScriptRunner scriptRunner, Configuration configuration, DatabaseMigrator databaseMigrator, DatabaseCreator databaseCreator, DatabaseGenerationOptions options)
        {
            ScriptRunner = scriptRunner ?? throw new ArgumentNullException(nameof(scriptRunner));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            DatabaseMigrator = databaseMigrator ?? throw new ArgumentNullException(nameof(databaseMigrator));
            DatabaseCreator = databaseCreator ?? throw new ArgumentNullException(nameof(databaseCreator));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     Will generate a database based on the options configured.
        /// </summary>
        /// <remarks>
        ///     Was done since this logic was being repeatidly copy & pasted.
        /// </remarks>
        public void Run()
        {
            using (var sessionFactory = Configuration.BuildSessionFactory())
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                if (Options.PreGenerationScriptBin != null)
                {
                    ScriptRunner.RunScriptsInDirectory(Options.PreGenerationScriptBin);
                }

                if (Options.Migrate)
                {
                    if (Options.DryRun)
                    {
                        DatabaseMigrator.DryRun();
                    }
                    else
                    {
                        DatabaseMigrator.Migrate();
                    }
                }
                else
                {
                    if (Options.DryRun)
                    {
                        DatabaseCreator.DryRun();
                    }
                    else
                    {
                        DatabaseCreator.Create();
                    }
                }

                if (Options.PostGenerationScriptBin != null)
                {
                    ScriptRunner.RunScriptsInDirectory(Options.PostGenerationScriptBin);
                }

                transaction.Commit();
            }
        }
    }
}
