using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class DomainOptionsTests
    {
        [Fact]
        public void It_will_have_parameterless_constructor()
        {
            var parameterlessConstructor = typeof(DomainOptions).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_has_constructor_with_iconfiguration_parameter()
        {
            var configurationConstructor = typeof(DomainOptions).GetConstructor(new[] { typeof(IConfiguration) });
            Assert.NotNull(configurationConstructor);
        }

        [Fact]
        public void It_throws_argument_null_exception_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainOptions(null));
        }

        [Fact]
        public void It_binds_connection_string()
        {
            var connectionString = "Server=.;Instance=test;Integrated_Security=true";
            var configurationOptions = new Dictionary<string, string>
            {
                [$"domain:{nameof(DomainOptions.ConnectionString)}"] = connectionString
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationOptions).Build();

            var options = new DomainOptions(configuration);

            Assert.Equal(options.ConnectionString, connectionString);
        }
    }
}