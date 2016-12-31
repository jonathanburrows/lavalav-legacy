using lvl.DatabaseGenerator;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Repositories.Tests.Fixtures
{
    public class OracleRepositoryFixture : RepositoryFixture
    {
        public OracleRepositoryFixture() : base(TestConnections.Oracle())
        {
            var databaseCreator = ServiceProvider.GetRequiredService<DatabaseCreator>();
            databaseCreator.Create();
        }
    }
}
