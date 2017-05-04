using lvl.Web.Logging;
using lvl.Web.OData;
using lvl.Web.Serialization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class WebServicesTests
    {
        private IServiceProvider Services { get; }

        public WebServicesTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));
        }

        [Fact]
        public void AfterAddingWeb_ResolvingEntityDeserializer_ReturnsValue()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();

            Assert.NotNull(entityDeserializer);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingILoggerProvider_ReturnsValue()
        {
            var loggerProvider = Services.GetRequiredService<ILoggerProvider>();

            Assert.NotNull(loggerProvider);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingILoggerFactory_ReturnsValue()
        {
            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            Assert.NotNull(loggerFactory);
        }

        [Fact]
        public void WhenAddingWeb_AddCorsIsCalled()
        {
            var corsService = Services.GetRequiredService<ICorsService>();

            Assert.NotNull(corsService);
        }

        [Fact]
        public void WhenAddingWeb_CallsAddMvc()
        {
            var controllerFactory = Services.GetRequiredService<IControllerFactory>();

            Assert.NotNull(controllerFactory);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingLoggingSettings_ReturnsValue()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();

            Assert.NotNull(loggingSettings);
        }

        [Fact]
        public void WhenAddingWeb_WithoutDomains_InvalidOperationExceptionIsThrown()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => serviceCollection.AddWeb());
        }

        [Fact]
        public void WhenAddingWebWithoutRepositories_InvalidOperationExceptionIsThrown()
        {
            var serviceCollection = new ServiceCollection().AddDomains();

            Assert.Throws<InvalidOperationException>(() => serviceCollection.AddWeb());
        }

        [Fact]
        public void AfterAddingWeb_ResolvingODataQueryParser_ReturnsValue()
        {
            var odataParser = Services.GetRequiredService<ODataQueryParser>();

            Assert.NotNull(odataParser);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingODataConventionTokenizer_ReturnsValue()
        {
            var conventionTokenizer = Services.GetRequiredService<ODataConventionTokenizer>();

            Assert.NotNull(conventionTokenizer);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingODataConventionParser_ReturnsValue()
        {
            var conventionParser = Services.GetRequiredService<ODataConventionParser>();

            Assert.NotNull(conventionParser);
        }
    }
}
