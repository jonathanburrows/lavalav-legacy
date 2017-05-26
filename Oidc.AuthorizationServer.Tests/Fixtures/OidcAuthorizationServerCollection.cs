using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests.Fixtures
{
    [CollectionDefinition(nameof(OidcAuthorizationServerCollection))]
    public class OidcAuthorizationServerCollection : 
        ICollectionFixture<ServiceProviderFixture>,
        ICollectionFixture<WebHostFixture<Startup>>
    {
    }
}
