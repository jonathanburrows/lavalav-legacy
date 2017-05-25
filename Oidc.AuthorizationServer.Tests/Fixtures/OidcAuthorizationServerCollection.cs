using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class OidcAuthorizationServerCollection : 
        ICollectionFixture<ServiceProviderFixture>,
        ICollectionFixture<WebHostFixture<Startup>>
    {
        public const string Name = nameof(OidcAuthorizationServerCollection);
    }
}
