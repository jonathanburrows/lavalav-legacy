using lvl.TestWebSite.Fixtures;
using Xunit;

namespace lvl.Web.Tests.Fixtures
{
    [CollectionDefinition(nameof(WebCollection))]
    public class WebCollection : 
        ICollectionFixture<WebServiceProviderFixture>,
        ICollectionFixture<WebHostFixture<TestWebSite.Startup>>,
        ICollectionFixture<WebHostFixture<CorsTests.CorsStartup>>
    {
    }
}
