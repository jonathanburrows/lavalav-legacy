using lvl.DatabaseGenerator;
using NHibernate;
using NHibernate.Cfg;
using System.Data.SQLite;
using System.Data;

namespace lvl.Repositories
{
    /// <summary>
    /// Provides a way for the sql lite database to not regenerate every request.
    /// </summary>
    // ReSharper disable once InconsistentNaming Is the literal name of the vendor.
    internal sealed class SQLitePersistentSessionProvider : SessionProvider
    {
        private static object ConnectionLock { get; } = new object();
        private static IDbConnection Connection { get; set; }

        public SQLitePersistentSessionProvider(ISessionFactory sessionFactory, DatabaseCreator databaseCreator, Configuration configuration) : base(sessionFactory)
        {
            if (sessionFactory == null)
            {
                throw new System.ArgumentNullException(nameof(sessionFactory));
            }

            if (Connection == null)
            {
                lock (ConnectionLock)
                {
                    if (Connection == null)
                    {
                        var connectionStringKey = Environment.ConnectionString;
                        var connectionString = configuration.GetProperty(connectionStringKey);
                        Connection = new SQLiteConnection(connectionString);
                        Connection.Open();
                        databaseCreator.Create(GetSession());
                    }
                }
            }

        }

        /// <inheritdoc />
        public override ISession GetSession() => SessionFactory.OpenSession(Connection);
    }
}
