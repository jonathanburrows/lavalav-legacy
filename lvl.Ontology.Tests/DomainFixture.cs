using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Ontology.Tests
{
    public abstract class DomainFixture
    {
        internal IServiceProvider Services { get; }

        public DomainFixture(string connectionString)
        {
            Services = new ServiceCollection()
                .AddDomains()
                .BuildServiceProvider();
        }
    }

    public class InMemoryDomainFixture : DomainFixture
    {
        public InMemoryDomainFixture() : base(null) { }
    }
}
