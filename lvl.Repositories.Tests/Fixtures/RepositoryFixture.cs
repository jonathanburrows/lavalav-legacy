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
                .AddDomains(connectionString)
                .AddDatabaseGeneration()
                .AddRepositories()
                .BuildServiceProvider();
        }
    }
}
