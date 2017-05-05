using lvl.Ontology.Conventions;
using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class MaxLengthConventionTests
    {
        private PersistentClass ClassMapping { get; }

        public MaxLengthConventionTests(InMemoryDomainFixture domainFixture)
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
            ClassMapping = configuration.GetClassMapping(typeof(MaxLengthConventionPoco));
            if (ClassMapping == null)
            {
                throw new InvalidOperationException($"{nameof(MaxLengthConventionPoco)} was not properly registered.");
            }
        }

        [Fact]
        public void It_sets_default_length_when_no_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(MaxLengthConventionPoco.PropertyWithoutAttribute));
            var column = (Column)property.ColumnIterator.Single();

            Assert.Equal(column.Length, MaxLengthConvention.DefaultLength);
        }

        [Fact]
        public void It_sets_attribute_length_when_specified()
        {
            var property = ClassMapping.GetProperty(nameof(MaxLengthConventionPoco.PropertyWithAttribute));
            var column = (Column)property.ColumnIterator.Single();

            Assert.Equal(column.Length, 1231);
        }

        public class MaxLengthConventionPoco : IEntity
        {
            public int Id { get; set; }

            public string PropertyWithoutAttribute { get; set; }

            [MaxLength(1231)]
            public string PropertyWithAttribute { get; set; }
        }
    }
}
