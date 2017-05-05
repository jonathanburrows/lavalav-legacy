using lvl.Ontology;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.DatabaseGenerator.Tests.Fixtures
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
