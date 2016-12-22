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
    public sealed class SQLitePersistentSessionManager : SessionManager
    {
        private static object connectionLock { get; } = new object();
        private static IDbConnection Connection { get; set; }

        public SQLitePersistentSessionManager(ISessionFactory sessionFactory, DatabaseCreator databaseCreator, Configuration configuration) : base(sessionFactory)
        {
            if (Connection == null)
            {
                lock (connectionLock)
                {
                    if (Connection == null)
                    {
                        var connectionStringKey = Environment.ConnectionString;
                        var connectionString = configuration.GetProperty(connectionStringKey);
                        Connection = new SQLiteConnection(connectionString);
                        Connection.Open();
                        databaseCreator.Create(OpenSession());
                    }
                }
            }

        }

        public override ISession OpenSession() => SessionFactory.OpenSession(Connection);
    }
}
