using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace lvl.Web.Tests
{
    public class WebServicesTests : IClassFixture<WebServiceProviderFixture>
    {
        private IServiceProvider Services { get; }

        public WebServicesTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture.ServiceProvider;
        }

        [Fact]
        public void WhenAddingWeb_CallsAddMvc()
        {
            var controllerFactory = Services.GetRequiredService<IControllerFactory>();

            Assert.NotNull(controllerFactory);
        }
    }
}
