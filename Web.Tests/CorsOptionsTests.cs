using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class CorsOptionsTests
    {
        [Fact]
        public void It_has_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(CorsOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_parameter_with_configuration_as_parameter()
        {
            var configurationConstructor = typeof(CorsOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_if_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CorsOptions(null));
        }

        [Fact]
        public void It_constructs_if_cors_section_is_missing()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new CorsOptions(configuration);

            Assert.NotNull(options);
        }

        [Fact]
        public void It_has_empty_headers_if_not_specified()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new CorsOptions(configuration);

            Assert.Empty(options.AllowHeaders);
        }

        [Fact]
        public void It_binds_one_header()
        {
            var header = "content-type";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowHeaders)}:0"] = header
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowHeaders.Single(), header);
        }

        [Fact]
        public void It_binds_two_headers()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowHeaders)}:0"] = "content-type",
                [$"cors:{nameof(CorsOptions.AllowHeaders)}:1"] = "data-type"
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowHeaders.Count(), 2);
        }

        [Fact]
        public void It_has_empty_methods_if_not_specified()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new CorsOptions(configuration);

            Assert.Empty(options.AllowMethods);
        }

        [Fact]
        public void It_binds_one_method()
        {
            var method = "GET";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowMethods)}:0"] = method
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowMethods.Single(), method);
        }

        [Fact]
        public void It_binds_two_methods()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowMethods)}:0"] = "GET",
                [$"cors:{nameof(CorsOptions.AllowMethods)}:1"] = "POST"
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowMethods.Count(), 2);
        }

        [Fact]
        public void It_has_empty_origins_if_not_specified()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new CorsOptions(configuration);

            Assert.Empty(options.AllowOrigins);
        }

        [Fact]
        public void It_binds_one_origin()
        {
            var origin = "http://localhost";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowOrigins)}:0"] = origin
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowOrigins.Single(), origin);
        }

        [Fact]
        public void It_binds_two_origins()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.AllowOrigins)}:0"] = "http://localhost:8080",
                [$"cors:{nameof(CorsOptions.AllowOrigins)}:1"] = "http://localhost:8081"
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.AllowOrigins.Count(), 2);
        }

        [Fact]
        public void It_has_empty_exposed_headers_if_not_specified()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var options = new CorsOptions(configuration);

            Assert.Empty(options.ExposedHeaders);
        }

        [Fact]
        public void It_binds_one_exposed_header()
        {
            var exposedHeader = "authority";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.ExposedHeaders)}:0"] = exposedHeader
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.ExposedHeaders.Single(), exposedHeader);
        }

        [Fact]
        public void It_binds_two_exposed_headers()
        {
            var configurationOptions = new Dictionary<string, string>
            {
                [$"cors:{nameof(CorsOptions.ExposedHeaders)}:0"] = "authority",
                [$"cors:{nameof(CorsOptions.ExposedHeaders)}:1"] = "custom-header"
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new CorsOptions(configuration);

            Assert.Equal(options.ExposedHeaders.Count(), 2);
        }
    }
}
