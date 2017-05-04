using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class DatabaseDetectorTests
    {
        private DatabaseDetector DatabaseDetector { get; }

        public DatabaseDetectorTests(DatabaseDetector databaseDetector)
        {
            DatabaseDetector = databaseDetector ?? throw new ArgumentNullException(nameof(databaseDetector));
        }

        [Fact]
        public void WhenDetecting_WhenConnectionStringIsNull_SQLiteIsReturned()
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(null);
            Assert.Equal(databaseVendor, DatabaseVendor.SQLite);
        }

        [Theory]
        [InlineData(@"Server=.;Database=lvl;User Id=admin;Password=password;")]
        [InlineData(@"Server=.;Database=lvl;Trusted_Connection=True;")]
        [InlineData(@"Data Source=.;Initial Catalog=lvl;Integrated Security=SSPI;User ID=.\admin;Password=password;")]
        public void WhenDetecting_WhenConnectionStringIsMsSql_MsSqlIsReturned(string msSqlConnectionString)
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(msSqlConnectionString);
            Assert.Equal(databaseVendor, DatabaseVendor.MsSql);
        }

        [Theory]
        [InlineData(@"Data Source=lvl;Integrated Security=yes;")]
        [InlineData(@"Data Source=lvl;User Id=admin;Password=password;Integrated Security=no;")]
        public void WhenDetecting_WhenConnectionStringIsOracle_OracleIsReturned(string oracleConnectionString)
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(oracleConnectionString);
            Assert.Equal(databaseVendor, DatabaseVendor.Oracle);
        }

        [Fact]
        public void WhenDetecting_IfNoMatchingVendor_ReturnsUnsupported()
        {
            var invalidConnectionString = "Hello, world!";
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(invalidConnectionString);

            Assert.Equal(databaseVendor, DatabaseVendor.Unsupported);
        }

        [Fact]
        public void WhenDetecting_WhenSQLiteIsConfigured_SQLiteIsReturned()
        {
            var services = new ServiceCollection().AddDomains().BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var databaseVendor = DatabaseDetector.GetConfigurationsVendor(configuration);

            Assert.Equal(databaseVendor, DatabaseVendor.SQLite);
        }

        [Fact]
        public void WhenDetecting_WhenMsSqlIsConfigured_MsSqlIsReturned()
        {
            var msSqlConnectionString = @"Server=.;Database=lvl;Trusted_Connection=True;";
            var services = new ServiceCollection().AddDomains(msSqlConnectionString).BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var databaseVendor = DatabaseDetector.GetConfigurationsVendor(configuration);

            Assert.Equal(DatabaseVendor.MsSql, databaseVendor);
        }

        [Fact]
        public void WhenDetecting_WhenOracleIsConfigured_OracleIsReturned()
        {
            var oracleConnectionString = @"Data Source=lvl;Integrated Security=yes;";
            var services = new ServiceCollection().AddDomains(oracleConnectionString).BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var databaseVendor = DatabaseDetector.GetConfigurationsVendor(configuration);

            Assert.Equal(databaseVendor, DatabaseVendor.Oracle);
        }
    }
}
