using lvl.DatabaseGenerator;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Repositories.Tests
{
    public abstract class RepositoryFixture
    {
        public IServiceProvider ServiceProvider { get; }

        public RepositoryFixture(string connectionString)
        {
            ServiceProvider = new ServiceCollection()
                .AddDomains(connectionString)
                .AddDatabaseGeneration()
                .AddRepositories()
                .BuildServiceProvider();
        }
    }

    public class SQLiteRepositoryFixture : RepositoryFixture
    {
        public SQLiteRepositoryFixture() : base(null) { }
    }

    public class MsSqlRepositoryFixture : RepositoryFixture
    {
        public MsSqlRepositoryFixture() : base(TestConnections.MsSql())
        {
            var databaseMigrator = ServiceProvider.GetRequiredService<DatabaseMigrator>();
            databaseMigrator.Migrate();
        }
    }

    public class OracleRepositoryFixture : RepositoryFixture
    {
        public OracleRepositoryFixture() : base(TestConnections.Oracle())
        {
            var databaseMigrator = ServiceProvider.GetRequiredService<DatabaseCreator>();
            databaseMigrator.Create();
        }
    }
}
