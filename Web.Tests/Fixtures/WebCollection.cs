using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Web.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class WebCollection : 
        ICollectionFixture<WebServiceProviderFixture>,
        ICollectionFixture<WebHostFixture<TestWebSite.Startup>>,
        ICollectionFixture<WebHostFixture<CorsTests.CorsStartup>>
    {
        public const string Name = nameof(WebCollection);
    }
}
