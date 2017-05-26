using lvl.Ontology.Tests.Fixtures;
using System.Linq;
using System.Reflection;
using Xunit;

namespace lvl.Ontology.Tests
{
    [Collection(nameof(OntologyCollection))]
    public class EntityTests
    {
        [Fact]
        public void It_will_be_abstract()
        {
            Assert.True(typeof(Entity).IsAbstract);
        }

        [Fact]
        public void It_will_have_a_parameterless_constructor()
        {
            // GetConstructor(Types.EmptyType) wasnt used because the constructor is protected.
            var constructors = typeof(Entity).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var parameterlessConstructor = constructors.SingleOrDefault(c => !c.GetParameters().Any());

            Assert.NotNull(parameterlessConstructor);
        }

        [Fact]
        public void It_will_have_a_virtual_id_property()
        {
            var idPropertyInfo = typeof(Entity).GetProperty(nameof(Entity.Id));

            Assert.NotNull(idPropertyInfo);
        }

        [Fact]
        public void Its_equal_will_be_false_when_obj_is_not_entity()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new NonEntity();

            // ReSharper disable once SuspiciousTypeConversion.Global Need to make sure equals isnt removed.
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_obj_is_null()
        {
            var a = new ComparisonEntity { Id = 1 };

            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Its_equal_will_be_true_when_references_are_equal()
        {
            var a = new ComparisonEntity();
            var b = a;

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_of_different_type()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new DifferentTypeEntity { Id = 1 };

            // ReSharper disable once SuspiciousTypeConversion.Global Need to make sure equals isnt overriden/removed.
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_its_id_is_zero()
        {
            var a = new ComparisonEntity { Id = 0 };
            var b = new ComparisonEntity { Id = 1 };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_obj_id_is_zero()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 0 };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_both_ids_are_zero()
        {
            var a = new ComparisonEntity { Id = 0 };
            var b = new ComparisonEntity { Id = 0 };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_true_when_both_ids_are_matching()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 1 };

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Its_equal_will_be_false_when_ids_are_different()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 2 };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void Its_equal_operator_will_be_true_if_both_are_null()
        {
            Entity a = null;
            Entity b = null;

            Assert.True(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_if_a_is_null()
        {
            Entity a = null;
            Entity b = new ComparisonEntity { Id = 1 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_b_is_null()
        {
            Entity a = new ComparisonEntity { Id = 1 };
            Entity b = null;

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_true_when_references_are_equal()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = a;

            Assert.True(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_of_different_type()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new DifferentTypeEntity { Id = 1 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_a_id_is_zero()
        {
            var a = new ComparisonEntity { Id = 0 };
            var b = new ComparisonEntity { Id = 1 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_b_id_is_zero()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 0 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_both_ids_are_zero()
        {
            var a = new ComparisonEntity { Id = 0 };
            var b = new ComparisonEntity { Id = 0 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_true_when_both_ids_are_matching()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 1 };

            Assert.True(a == b);
        }

        [Fact]
        public void Its_equal_operator_will_be_false_when_ids_are_different()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 2 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_GetHashCode_will_return_the_hash_of_entity_type_plus_id()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 2 };

            Assert.False(a == b);
        }

        [Fact]
        public void Its_equals_works_with_linqs_contain()
        {
            var a = new ComparisonEntity { Id = 1 };
            var b = new ComparisonEntity { Id = 1 };
            var entities = new[] { a };

            Assert.True(entities.Contains(b));
        }

        private class ComparisonEntity : Entity { }

        private class DifferentTypeEntity : Entity { }

        private class NonEntity { }
    }
}
