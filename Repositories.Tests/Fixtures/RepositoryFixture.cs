using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Repositories.Tests.Fixtures
{
    public abstract class RepositoryFixture
    {
        public IServiceProvider ServiceProvider { get; }

        protected RepositoryFixture(string connectionString)
        {
            ServiceProvider = new ServiceCollection()
                .AddDomains(new DomainOptions { ConnectionString = connectionString })
                .AddDatabaseGeneration()
                .AddRepositories()
                .BuildServiceProvider();
        }
    }
}
