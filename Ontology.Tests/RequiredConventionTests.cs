using lvl.Ontology.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(OntologyCollection.Name)]
    public class RequiredConventionTests
    {
        private PersistentClass ClassMapping { get; }

        public RequiredConventionTests(InMemoryDomainFixture domainFixture)
        {
            var configuration = domainFixture.Services.GetRequiredService<Configuration>();
            ClassMapping = configuration.GetClassMapping(typeof(RequiredConventionPoco));
        }

        [Fact]
        public void It_sets_non_nullable_to_required_without_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.NonNullableWithoutAttribute));

            Assert.False(property.IsNullable);
        }

        [Fact]
        public void It_sets_non_nullable_to_required_with_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.NonNullableWithAttribute));

            Assert.False(property.IsNullable);
        }

        [Fact]
        public void It_sets_nullable_to_not_required_without_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.NullableWithoutAttribute));

            Assert.True(property.IsNullable);
        }

        [Fact]
        public void It_sets_nullable_to_required_with_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.NullableWithAttribute));

            Assert.False(property.IsNullable);
        }

        [Fact]
        public void It_sets_nullable_to_string_without_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.StringWithoutAttribute));

            Assert.True(property.IsNullable);
        }

        [Fact]
        public void It_sets_nullable_to_string_with_attribute()
        {
            var property = ClassMapping.GetProperty(nameof(RequiredConventionPoco.StringWithAttribute));

            Assert.False(property.IsNullable);
        }

        public class RequiredConventionPoco : IEntity
        {
            public int Id { get; set; }

            public int NonNullableWithoutAttribute { get; set; }

            [Required]
            public int NonNullableWithAttribute { get; set; }

            public int? NullableWithoutAttribute { get; set; }

            [Required]
            public int? NullableWithAttribute { get; set; }

            public string StringWithoutAttribute { get; set; }

            [Required]
            public string StringWithAttribute { get; set; }
        }
    }
}
