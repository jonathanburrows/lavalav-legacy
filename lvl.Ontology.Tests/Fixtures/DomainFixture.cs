using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Ontology.Tests.Fixtures
{
    public abstract class DomainFixture
    {
        internal IServiceProvider Services { get; }

        protected DomainFixture()
        {
            Services = new ServiceCollection()
                .AddDomains()
                .BuildServiceProvider();
        }
    }
}
