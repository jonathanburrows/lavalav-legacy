using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.DatabaseGenerator.Tests
{
    public class InMemoryDatabaseGenerationFixture
    {
        public IServiceProvider ServiceProvider { get; }

        public InMemoryDatabaseGenerationFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddDomains()
                .AddDatabaseGeneration()
                .BuildServiceProvider();
        }
    }
}
