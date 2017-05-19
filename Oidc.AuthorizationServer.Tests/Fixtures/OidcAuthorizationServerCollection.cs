using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class OidcAuthorizationServerCollection : ICollectionFixture<ServiceProviderFixture>
    {
        public const string Name = nameof(OidcAuthorizationServerCollection);
    }
}
