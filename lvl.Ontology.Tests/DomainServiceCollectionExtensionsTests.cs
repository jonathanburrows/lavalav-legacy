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
    [Collection(OntologyCollection.Name)]
    public class DomainServiceCollectionExtensionsTests
    {
        private IServiceProvider Services { get; }

        public DomainServiceCollectionExtensionsTests(InMemoryDomainFixture inMemoryTestFixture)
        {
            Services = inMemoryTestFixture?.Services ?? throw new ArgumentNullException(nameof(inMemoryTestFixture));
        }

        [Fact]
        public void AfterAddingDomains_WhenResolvingNHibernateConfig_ValueIsReturned()
        {
            var configuration = Services.GetRequiredService<Configuration>();

            Assert.NotNull(configuration);
        }

        [Fact]
        public void ModelsInReferencedAssemblies_WhenAddingDomains_AreAddedToNHibernateConfig()
        {
            var configuration = Services.GetRequiredService<Configuration>();
            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var referencedType = typeof(Moon);

            Assert.Contains(referencedType, mappedTypes);
        }

        [Fact]
        public void ModelsInExecutingAssembly_WhenAddingDomains_AreAddedToNHibernateConfig()
        {
            var configuration = Services.GetRequiredService<Configuration>();
            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var modelInExecutingClass = typeof(ModelInExecutingClass);

            Assert.Contains(modelInExecutingClass, mappedTypes);
        }

        [Fact]
        public void ModelsAlreadyAdded_WhenAddingDomains_AreNotAddedAgain()
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
        public void IfNoConnectionString_WhenAddingDomain_SqlLiteIsConfigured()
        {
            var services = new ServiceCollection().AddDomains().BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var driverKey = NHibernate.Cfg.Environment.ConnectionDriver;
            var driver = configuration.GetProperty(driverKey);

            var sqlLiteDriver = typeof(SQLite20Driver).AssemblyQualifiedName;
            Assert.Equal(sqlLiteDriver, driver);
        }

        [Theory]
        [InlineData(@"Server=.;Database=lvl;User Id=admin;Password=password;")]
        [InlineData(@"Server=.;Database=lvl;Trusted_Connection=True;")]
        [InlineData(@"Data Source=.;Initial Catalog=lvl;Integrated Security=SSPI;User ID=.\admin;Password=password;")]
        public void IfSqlServerConnectionString_WhenAddingDomain_SqlServerIsConfigured(string sqlServerConnectionString)
        {
            var services = new ServiceCollection()
                .AddDomains(new DomainOptions { ConnectionString = sqlServerConnectionString })
                .BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var driverKey = NHibernate.Cfg.Environment.ConnectionDriver;
            var driver = configuration.GetProperty(driverKey);

            var sqlServerDriver = typeof(SqlClientDriver).AssemblyQualifiedName;
            Assert.Equal(sqlServerDriver, driver);
        }

        [Theory]
        [InlineData(@"Data Source=lvl;Integrated Security=yes;")]
        [InlineData(@"Data Source=lvl;User Id=admin;Password=password;Integrated Security=no;")]
        public void IfOracleConnectionString_WhenAddingDomain_OracleIsConfigured(string oracleConnectionString)
        {
            var services = new ServiceCollection()
                .AddDomains(new DomainOptions { ConnectionString = oracleConnectionString })
                .BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var driverKey = NHibernate.Cfg.Environment.ConnectionDriver;
            var driver = configuration.GetProperty(driverKey);

            var oracleDriver = typeof(OracleManagedDataClientDriver).AssemblyQualifiedName;
            Assert.Equal(oracleDriver, driver);
        }

        [Theory]
        [InlineData("not a connection string")]
        public void IfInvalidConnectionString_WhenAddingDomain_ArgumentExceptionIsThrown(string invalidConnectionString)
        {
            var serviceCollection = new ServiceCollection();
            var domainOptions = new DomainOptions { ConnectionString = invalidConnectionString };
            Assert.Throws<ArgumentException>(() => serviceCollection.AddDomains(domainOptions));
        }

        /// <summary>Used to test classes embedded in application</summary>
        public class ModelInExecutingClass : IEntity
        {
            public int Id { get; set; }
        }
    }
}