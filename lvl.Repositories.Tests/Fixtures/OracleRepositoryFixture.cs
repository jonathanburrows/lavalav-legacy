using lvl.DatabaseGenerator;
using lvl.Repositories.Tests.Configuration;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Repositories.Tests.Fixtures
{
    public class OracleRepositoryFixture : RepositoryFixture
    {
        public OracleRepositoryFixture() : base(ConfigurationReader.IntegrationSettings.Oracle.ConnectionString)
        {
            var databaseCreator = ServiceProvider.GetRequiredService<DatabaseCreator>();
            databaseCreator.Create();
        }
    }
}
