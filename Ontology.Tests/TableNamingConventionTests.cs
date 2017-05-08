using lvl.Ontology.Conventions;
using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class TableNamingConventionTests
    {
        private Configuration Configuration { get; }

        public TableNamingConventionTests(InMemoryDomainFixture domainFixture)
        {
            Configuration = domainFixture.Services.GetRequiredService<Configuration>();
        }

        [Fact]
        public void It_uses_defaults_when_no_attributes()
        {
            var mapping = Configuration.GetClassMapping(typeof(NoTableNameOrSchema));

            Assert.Equal(mapping.Table.Name, $"lvl_{nameof(NoTableNameOrSchema)}");
        }

        public class NoTableNameOrSchema : Entity { }

        [Fact]
        public void It_uses_custom_schema_attribute()
        {
            var mapping = Configuration.GetClassMapping(typeof(NoTableNameButSchema));

            Assert.Equal(mapping.Table.Name, $"Schema_{nameof(NoTableNameButSchema)}");
        }

        [Schema("Schema")]
        public class NoTableNameButSchema : Entity { }

        [Fact]
        public void It_uses_table_attribute()
        {
            var mapping = Configuration.GetClassMapping(typeof(TableNameNoSchema));

            Assert.Equal(mapping.Table.Name, $"lvl_TableOnly");
        }

        [Table("TableOnly")]
        public class TableNameNoSchema : Entity { }

        [Fact]
        public void It_can_set_both_table_and_schema()
        {
            var mapping = Configuration.GetClassMapping(typeof(BothTableNameAndSchema));

            Assert.Equal(mapping.Table.Name, $"Schema_Table");
        }

        [Table("Table", Schema = "Schema")]
        public class BothTableNameAndSchema : Entity { }

        [Fact]
        public void Table_attribute_overrides_custom_schema()
        {
            var mapping = Configuration.GetClassMapping(typeof(TableOverridesSchema));

            Assert.Equal(mapping.Table.Name, $"Override_MyTable");
        }

        [Table("MyTable", Schema = "Override"), Schema("Overridden")]
        public class TableOverridesSchema : Entity { }
    }
}
