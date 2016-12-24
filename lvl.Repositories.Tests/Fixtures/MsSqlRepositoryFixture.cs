using lvl.DatabaseGenerator;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Repositories.Tests.Fixtures
{
    public class MsSqlRepositoryFixture : RepositoryFixture
    {
        public MsSqlRepositoryFixture() : base(TestConnections.MsSql())
        {
            var databaseMigrator = ServiceProvider.GetRequiredService<DatabaseMigrator>();
            databaseMigrator.Migrate();
        }
    }
}
