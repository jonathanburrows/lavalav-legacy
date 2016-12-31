using lvl.DatabaseGenerator;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace lvl.Repositories.Tests.Fixtures
{
    public class MsSqlRepositoryFixture : RepositoryFixture
    {
        public MsSqlRepositoryFixture() : base(TestConnections.MsSql())
        {
            var databaseCreator = ServiceProvider.GetRequiredService<DatabaseCreator>();
            databaseCreator.Create();
        }
    }
}
