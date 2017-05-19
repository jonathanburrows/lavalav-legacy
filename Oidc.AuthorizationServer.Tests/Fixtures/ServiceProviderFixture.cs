using Microsoft.Extensions.DependencyInjection;
using System;

namespace lvl.Oidc.AuthorizationServer.Tests.Fixtures
{
    public class ServiceProviderFixture
    {
        public IServiceProvider ServiceProvider { get; }

        public ServiceProviderFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer()
                .BuildServiceProvider();
        }
    }
}
