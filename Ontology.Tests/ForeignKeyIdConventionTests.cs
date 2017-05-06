using lvl.Ontology.Conventions;
using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System.Linq;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class ForeignKeyIdConventionTests
    {
        private Configuration Configuration { get; }

        public ForeignKeyIdConventionTests(InMemoryDomainFixture domainFixture)
        {
            Configuration = domainFixture.Services.GetRequiredService<Configuration>();
        }

        [Fact]
        public void It_adds_foreign_key_to_child()
        {
            var mapping = Configuration.GetClassMapping(typeof(SingleChildParent));

            var foreignKeys = mapping.RootTable.ForeignKeyIterator;
            Assert.True(foreignKeys.Any(key => key.ReferencedEntityName == typeof(ChildSon).FullName));
        }

        public class SingleChildParent : IEntity
        {
            public int Id { get; set; }

            [ForeignKeyId(typeof(ChildSon))]
            public int ChildSonId { get; set; }
        }

        [Fact]
        public void It_can_set_two_foreign_keys()
        {
            var mapping = Configuration.GetClassMapping(typeof(DoubleChildParent));

            var foreignKeys = mapping.RootTable.ForeignKeyIterator;
            Assert.Equal(foreignKeys.Count(), 2);
        }

        public class DoubleChildParent : IEntity
        {
            public int Id { get; set; }

            [ForeignKeyId(typeof(ChildSon))]
            public int ChildSonId { get; set; }

            [ForeignKeyId(typeof(ChildDaughter))]
            public int ChildDaughterId { get; set; }
        }

        [Fact]
        public void It_can_set_two_foreign_keys_to_same_entity()
        {
            var mapping = Configuration.GetClassMapping(typeof(DoubleChildSameClassParent));

            var entityReferences = mapping.RootTable.ForeignKeyIterator.Where(fk => fk.ReferencedEntityName == typeof(ChildSon).FullName);
            Assert.Equal(entityReferences.Count(), 2);
        }

        public class DoubleChildSameClassParent : IEntity
        {
            public int Id { get; set; }

            [ForeignKeyId(typeof(ChildSon))]
            public int ChildSonId { get; set; }

            [ForeignKeyId(typeof(ChildSon))]
            public int SecondChildSonId { get; set; }
        }

        [Fact]
        public void It_can_allow_self_referencing_ids()
        {
            var mapping = Configuration.GetClassMapping(typeof(SelfReferencingParent));

            var foreignKeys = mapping.RootTable.ForeignKeyIterator;
            Assert.True(foreignKeys.Any(key => key.ReferencedEntityName == typeof(SelfReferencingParent).FullName));
        }

        public class SelfReferencingParent : IEntity
        {
            public int Id { get; set; }

            [ForeignKeyId(typeof(SelfReferencingParent))]
            public int? SelfReferencingParentId { get; set; }
        }

        [Fact]
        public void It_allows_naming_of_keys_against_convention()
        {
            var mapping = Configuration.GetClassMapping(typeof(UniquelyNamedParent));

            var uniqueKey = mapping.RootTable.ForeignKeyIterator.Single(key => key.ReferencedEntityName == typeof(ChildSon).FullName);
            var column = uniqueKey.Columns.Single();
            Assert.Equal(column.Name, nameof(UniquelyNamedParent.HelloWorld));
        }

        public class UniquelyNamedParent : IEntity
        {
            public int Id { get; set; }

            [ForeignKeyId(typeof(ChildSon))]
            public int HelloWorld { get; set; }
        }

        public class ChildSon : IEntity
        {
            public int Id { get; set; }
        }

        public class ChildDaughter : IEntity
        {
            public int Id { get; set; }
        }
    }
}
