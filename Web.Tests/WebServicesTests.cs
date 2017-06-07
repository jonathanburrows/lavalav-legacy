using lvl.Web.Authorization;
using lvl.Web.OData;
using lvl.Web.Serialization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class WebServicesTests
    {
        private IServiceProvider Services { get; }

        public WebServicesTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));
        }

        [Fact]
        public void It_will_allow_entity_deserializer_to_resolve_entity_deserializer()
        {
            var entityDeserializer = Services.GetRequiredService<EntityDeserializer>();

            Assert.NotNull(entityDeserializer);
        }

        [Fact]
        public void It_will_allow_logger_provider_to_be_resolved()
        {
            var loggerProvider = Services.GetRequiredService<ILoggerProvider>();

            Assert.NotNull(loggerProvider);
        }

        [Fact]
        public void It_will_allow_logger_factory_to_be_resolved()
        {
            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            Assert.NotNull(loggerFactory);
        }

        [Fact]
        public void It_will_allow_cors_service_to_be_resolved()
        {
            var corsService = Services.GetRequiredService<ICorsService>();

            Assert.NotNull(corsService);
        }

        [Fact]
        public void It_will_allow_logging_options_to_be_resolved()
        {
            var loggingSettings = Services.GetRequiredService<LoggingOptions>();

            Assert.NotNull(loggingSettings);
        }

        [Fact]
        public void It_will_throw_invalid_operation_exception_when_adding_without_calling_domains()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => serviceCollection.AddWeb());
        }

        [Fact]
        public void It_will_throw_invalid_operation_exception_when_adding_without_calling_repositories()
        {
            var serviceCollection = new ServiceCollection().AddDomains();

            Assert.Throws<InvalidOperationException>(() => serviceCollection.AddWeb());
        }

        [Fact]
        public void It_will_allow_odata_query_parser_to_be_resolved()
        {
            var odataParser = Services.GetRequiredService<ODataQueryParser>();

            Assert.NotNull(odataParser);
        }

        [Fact]
        public void It_will_allow_odata_convention_tokenized_to_be_resolved()
        {
            var conventionTokenizer = Services.GetRequiredService<ODataConventionTokenizer>();

            Assert.NotNull(conventionTokenizer);
        }

        [Fact]
        public void It_will_allow_odata_convention_parser_to_be_resolved()
        {
            var conventionParser = Services.GetRequiredService<ODataConventionParser>();

            Assert.NotNull(conventionParser);
        }

        [Fact]
        public void It_will_allow_impersonator_to_be_resolved()
        {
            var impersonator = Services.GetRequiredService<Impersonator>();

            Assert.NotNull(impersonator);
        }
    }
}
