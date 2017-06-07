using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System;
using lvl.Ontology.Database;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(nameof(OntologyCollection))]
    public class DatabaseDetectorTests
    {
        private DatabaseDetector DatabaseDetector { get; }

        public DatabaseDetectorTests(DatabaseDetector databaseDetector)
        {
            DatabaseDetector = databaseDetector ?? throw new ArgumentNullException(nameof(databaseDetector));
        }

        [Fact]
        public void It_will_return_sql_lite_when_connection_string_is_null()
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(null);
            Assert.Equal(databaseVendor, DatabaseVendor.SQLite);
        }

        [Theory]
        [InlineData(@"Server=.;Database=lvl;User Id=admin;Password=password;")]
        [InlineData(@"Server=.;Database=lvl;Trusted_Connection=True;")]
        [InlineData(@"Data Source=.;Initial Catalog=lvl;Integrated Security=SSPI;User ID=.\admin;Password=password;")]
        public void It_will_detect_ms_sql_connection_strings(string msSqlConnectionString)
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(msSqlConnectionString);
            Assert.Equal(databaseVendor, DatabaseVendor.MsSql);
        }

        [Theory]
        [InlineData(@"Data Source=lvl;Integrated Security=yes;")]
        [InlineData(@"Data Source=lvl;User Id=admin;Password=password;Integrated Security=no;")]
        public void It_will_detect_oracle_connection_strings(string oracleConnectionString)
        {
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(oracleConnectionString);
            Assert.Equal(databaseVendor, DatabaseVendor.Oracle);
        }

        [Fact]
        public void It_will_return_unsupported_when_connection_string_is_invalid()
        {
            var invalidConnectionString = "Hello, world!";
            var databaseVendor = DatabaseDetector.GetConnectionStringsVendor(invalidConnectionString);

            Assert.Equal(databaseVendor, DatabaseVendor.Unsupported);
        }

        [Fact]
        public void It_will_set_the_configuration_connection_type()
        {
            var services = new ServiceCollection().AddDomains().BuildServiceProvider();
            var configuration = services.GetRequiredService<Configuration>();

            var databaseVendor = DatabaseDetector.GetConfigurationsVendor(configuration);

            Assert.Equal(databaseVendor, DatabaseVendor.SQLite);
        }
    }
}
