using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    /// Applies changes to a database to bring it up to the current model.
    /// </summary>
    public class DatabaseMigrator
    {
        private static object MigrationLock { get; } = new object();
        private Configuration Configuration { get; }

        public DatabaseMigrator(Configuration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Applies changes to the registered database, bringing it up the current model.
        /// </summary>
        /// <exception cref="System.AggregateException">There are breaking changes which prevent the migration.</exception>
        public void Migrate()
        {
            var migrator = new SchemaUpdate(Configuration);

            lock (MigrationLock)
            {
                migrator.Execute(true, true);
            }

            if (migrator.Exceptions.Any())
            {
                PrintExceptions(migrator.Exceptions);
                throw new AggregateException(migrator.Exceptions);
            }
        }

        /// <summary>
        /// Performs the changes without commiting them, then reports on any breaking changes 
        /// which prevent the database to coming up to the current model.
        /// </summary>
        public void DryRun()
        {
            var migrator = new SchemaUpdate(Configuration);
            migrator.Execute(true, false);
            PrintExceptions(migrator.Exceptions);
        }

        private void PrintExceptions(IEnumerable<Exception> exceptions)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var exception in exceptions)
            {
                Console.WriteLine(exception.Message);
            }
            Console.ResetColor();
        }
    }
}
