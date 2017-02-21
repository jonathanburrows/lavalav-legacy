using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;

namespace lvl.DatabaseGenerator
{
    /// <summary>
    /// Creates databases from scratch to match the registered model.
    /// </summary>
    public class DatabaseCreator
    {
        private static object CreateLock { get; } = new object();
        private Configuration Configuration { get; }

        public DatabaseCreator(Configuration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Creates a database which matches the registered model.
        /// </summary>
        public void Create()
        {
            var exporter = new SchemaExport(Configuration);
            lock (CreateLock)
            {
                exporter.Execute(true, true, false);
            }
        }

        /// <summary>
        /// This was done to allow for transactions to be run from the same configuration.
        /// </summary>
        /// <param name="session">The session which will provide the transaction.</param>
        /// <exception cref="ArgumentNullException"><paramref name="session"/> is null.</exception>
        public void Create(ISession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var exporter = new SchemaExport(Configuration);
            //this method was used because the same connection needs to be used for SQLite.
            exporter.Execute(true, true, false, session.Connection, null);
        }

        /// <summary>
        /// Reports on any potential issues which will prevent the database from being created.
        /// </summary>
        public void DryRun()
        {
            var exporter = new SchemaUpdate(Configuration);
            exporter.Execute(true, false);
            PrintExceptions(exporter.Exceptions);
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
