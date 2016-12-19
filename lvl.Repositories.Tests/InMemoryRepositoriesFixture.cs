using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Repositories.Tests
{
    public class InMemoryRepositoriesFixture
    {
        public IServiceProvider ServiceProvider { get; }

        public InMemoryRepositoriesFixture() {
            ServiceProvider = new ServiceCollection()
                .AddDomains()
                .AddRepositories()
                .BuildServiceProvider();
        }
    }
}
