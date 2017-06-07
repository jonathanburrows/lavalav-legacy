using lvl.Ontology.Tests.Fixtures;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Driver;
using System;
using System.Linq;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(nameof(OntologyCollection))]
    public class DomainServiceCollectionExtensionsTests
    {
        private IServiceProvider Services { get; }

        public DomainServiceCollectionExtensionsTests(InMemoryDomainFixture inMemoryTestFixture)
        {
            Services = inMemoryTestFixture?.Services ?? throw new ArgumentNullException(nameof(inMemoryTestFixture));
        }

        [Fact]
        public void It_will_allow_configuration_to_be_resolved()
        {
            var configuration = Services.GetRequiredService<Configuration>();

            Assert.NotNull(configuration);
        }

        [Fact]
        public void It_will_allow_domain_options_to_be_resolved()
        {
            var domainOptions = Services.GetRequiredService<DomainOptions>();

            Assert.NotNull(domainOptions);
        }

        [Fact]
        public void It_will_add_models_from_referenced_assemblies_to_nhibernate_config()
        {
            var configuration = Services.GetRequiredService<Configuration>();
            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var referencedType = typeof(Moon);

            Assert.Contains(referencedType, mappedTypes);
        }

        [Fact]
        public void It_will_add_models_from_executing_assembly_to_nhibernate_config()
        {
            var configuration = Services.GetRequiredService<Configuration>();
            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var modelInExecutingClass = typeof(ModelInExecutingClass);

            Assert.Contains(modelInExecutingClass, mappedTypes);
        }

        [Fact]
        public void It_will_not_add_model_from_referenced_assembly_twice()
        {
            var services = new ServiceCollection()
                .AddDomains()
                .AddDomains()
                .BuildServiceProvider();

            var configuration = services.GetRequiredService<Configuration>();
            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var moonType = typeof(Moon);

            Assert.Contains(moonType, mappedTypes);
        }

        [Fact]
        public void It_will_set_driver_to_sqlite_when_no_connection_string_given()
        {
            var services = new ServiceCollection().AddDomains().BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var driverKey = NHibernate.Cfg.Environment.ConnectionDriver;
            var driver = configuration.GetProperty(driverKey);

            var sqlLiteDriver = typeof(SQLite20Driver).AssemblyQualifiedName;
            Assert.Equal(sqlLiteDriver, driver);
        }

        [Theory]
        [InlineData("not a connection string")]
        public void It_will_throw_argument_exception_when_connection_string_is_invalid(string invalidConnectionString)
        {
            var domainOptions = new DomainOptions { ConnectionString = invalidConnectionString };
            var serviceCollection = new ServiceCollection();

            Assert.Throws<ArgumentException>(() => serviceCollection.AddDomains(domainOptions));
        }

        /// <summary>Used to test classes embedded in application</summary>
        public class ModelInExecutingClass : Entity
        {
        }
    }
}