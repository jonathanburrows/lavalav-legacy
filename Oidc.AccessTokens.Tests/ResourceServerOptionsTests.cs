using lvl.Oidc.AccessTokens.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace lvl.Oidc.AccessTokens.Tests
{
    [Collection(nameof(OidcAccessTokenCollection))]
    public class ResourceServerOptionsTests
    {
        [Fact]
        public void It_has_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(ResourceServerOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_parameter_with_configuration_as_parameter()
        {
            var configurationConstructor = typeof(ResourceServerOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_if_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceServerOptions(null));
        }

        [Fact]
        public void It_constructs_if_oidc_resource_server_is_missing()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new ResourceServerOptions(configuration);

            Assert.NotNull(options);
        }

        [Fact]
        public void It_sets_require_https_to_true_by_default()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new ResourceServerOptions(configuration);

            Assert.True(options.RequireHttpsMetadata);
        }

        [Fact]
        public void It_sets_require_http_to_false()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"oidc:resource-server:{nameof(ResourceServerOptions.RequireHttpsMetadata)}"] = "false",
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new ResourceServerOptions(configuration);

            Assert.False(options.RequireHttpsMetadata);
        }

        [Fact]
        public void It_sets_require_http_to_true()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"oidc:resource-server:{nameof(ResourceServerOptions.RequireHttpsMetadata)}"] = "true",
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new ResourceServerOptions(configuration);

            Assert.True(options.RequireHttpsMetadata);
        }

        [Fact]
        public void It_will_bind_authority()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"oidc:resource-server:{nameof(ResourceServerOptions.Authority)}"] = "my-authority",
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new ResourceServerOptions(configuration);

            Assert.Equal(options.Authority, "my-authority");
        }

        [Fact]
        public void It_will_bind_api_name()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"oidc:resource-server:{nameof(ResourceServerOptions.ApiName)}"] = "my-api",
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new ResourceServerOptions(configuration);

            Assert.Equal(options.ApiName, "my-api");
        }

        [Fact]
        public void It_will_bind_api_secret()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"oidc:resource-server:{nameof(ResourceServerOptions.ApiSecret)}"] = "my-secret",
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new ResourceServerOptions(configuration);

            Assert.Equal(options.ApiSecret, "my-secret");
        }
    }
}
