using lvl.Ontology.Conventions;
using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using System;
using System.Linq;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class UniqueNamingConventionTests
    {
        private PersistentClass ClassMapping { get; }

        public UniqueNamingConventionTests(InMemoryDomainFixture domainFixture)
        {
            if (domainFixture == null)
            {
                throw new ArgumentNullException(nameof(domainFixture));
            }
            if (domainFixture.Services == null)
            {
                throw new InvalidOperationException($"{nameof(domainFixture)}.{nameof(domainFixture.Services)} is null.");
            }

            var configuration = domainFixture.Services.GetRequiredService<Configuration>();
            ClassMapping = configuration.GetClassMapping(typeof(UniqueConventionPoco));
            if (ClassMapping == null)
            {
                throw new InvalidOperationException($"{nameof(UniqueConventionPoco)} was not properly registered.");
            }
        }

        [Fact]
        public void It_sets_property_as_unique_with_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(UniqueConventionPoco.Unique));
            var column = (Column)property.ColumnIterator.Single();

            Assert.True(column.Unique);
        }

        [Fact]
        public void It_sets_property_as_not_unique_without_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(UniqueConventionPoco.NonUnique));
            var column = (Column)property.ColumnIterator.Single();

            Assert.False(column.Unique);
        }

        public class UniqueConventionPoco : IEntity
        {
            public int Id { get; set; }

            [Unique]
            public string Unique { get; set; }

            public string NonUnique { get; set; }
        }
    }
}
