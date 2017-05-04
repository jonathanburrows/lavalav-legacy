using lvl.DatabaseGenerator;
using lvl.Repositories.Tests.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Repositories.Tests.Fixtures
{
    public class MsSqlRepositoryFixture : RepositoryFixture
    {
        public MsSqlRepositoryFixture() : base(ConfigurationReader.IntegrationSettings.MsSql.ConnectionString)
        {
            var databaseCreator = ServiceProvider.GetRequiredService<DatabaseCreator>();
            databaseCreator.Create();
        }
    }
}
