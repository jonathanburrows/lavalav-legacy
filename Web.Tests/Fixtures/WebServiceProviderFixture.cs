using System;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.Web.Tests.Fixtures
{
    public class WebServiceProviderFixture
    {
        public IServiceProvider ServiceProvider { get; }

        public WebServiceProviderFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddDomains(new DomainOptions())
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .BuildServiceProvider();
        }
    }
}
