using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Ontology.Tests
{
    public class InMemoryTestFixture
    {
        internal IServiceProvider Services { get; }

        public InMemoryTestFixture()
        {
            Services = new ServiceCollection()
                .AddDomains()
                .BuildServiceProvider();
        }
    }
}
