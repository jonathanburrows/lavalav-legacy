﻿using lvl.Web.Logging;
using lvl.Web.Serialization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebServicesTests))]
    public class WebServicesTests : IClassFixture<WebServiceProviderFixture>
    {
        private IServiceProvider Services { get; }

        public WebServicesTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture.ServiceProvider;
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
        public void WhenAddingWeb_CallsAddMvc()
        {
            var controllerFactory = Services.GetRequiredService<IControllerFactory>();

            Assert.NotNull(controllerFactory);
        }

        [Fact]
        public void AfterAddingWeb_ResolvingLoggerSettings_ReturnsValue()
        {
            var loggerSettings = Services.GetRequiredService<LoggerSettings>();

            Assert.NotNull(loggerSettings);
        }

        [Fact]
        public void WhenAddingWeb_AndWebSettingsIsNull_ArgumentNullExceptionIsThrown()
        {
            var serviceCollection = new ServiceCollection().AddDomains().AddRepositories();

            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddWeb(null));
        }

        [Fact]
        public void WhenAddingWeb_AndLoggerSettingsIsNull_ArgumentNullExceptionIsThrown()
        {
            var serviceCollection = new ServiceCollection().AddDomains().AddRepositories();
            var webSettings = new WebSettings
            {
                LoggerSettings = null
            };

            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddWeb(webSettings));
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
    }
}
