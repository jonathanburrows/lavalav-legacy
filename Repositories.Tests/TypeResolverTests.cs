using lvl.Ontology;
using lvl.Repositories.Tests.Configuration;
using lvl.Repositories.Tests.Fixtures;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace lvl.Repositories.Tests
{
    public abstract class TypeResolverTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture
    {
        private IServiceProvider Services { get; }

        protected TypeResolverTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryRepositoriesFixture));
        }

        [IntegrationTest]
        public void It_will_return_type_when_using_fully_qualified_name()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var resolvedType = typeResolver.Resolve(moonType.FullName);

            Assert.Equal(resolvedType, moonType);
        }

        [IntegrationTest]
        public void It_will_return_type_when_given_full_equalified_name_with_wrong_case()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var mangedName = new StringBuilder(moonType.FullName);
            for (var i = 0; i < mangedName.Length; i++)
            {
                if (i % 2 == 0)
                {
                    mangedName[i] = mangedName[i].ToString().ToUpper().ToCharArray().Single();
                }
            }

            var resolvedType = typeResolver.Resolve(mangedName.ToString());

            Assert.Equal(moonType, resolvedType);
        }

        [IntegrationTest]
        public void It_will_return_type_when_given_name_of_entity()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var resolvedType = typeResolver.Resolve(moonType.Name);

            Assert.Equal(resolvedType, moonType);
        }

        [IntegrationTest]
        public void It_will_return_type_when_given_name_of_entity_with_wrong_case()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var mangedName = new StringBuilder(moonType.Name);
            for (var i = 0; i < mangedName.Length; i++)
            {
                if (i % 2 == 0)
                {
                    mangedName[i] = mangedName[i].ToString().ToUpper().ToCharArray().Single();
                }
            }

            var resolvedType = typeResolver.Resolve(mangedName.ToString());

            Assert.Equal(moonType, resolvedType);
        }

        [IntegrationTest]
        public void It_will_throw_argument_null_exception_when_name_is_null()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            Assert.Throws<ArgumentNullException>(() => typeResolver.Resolve(null));
        }

        [IntegrationTest]
        public void It_will_invalid_operation_exception_when_two_entities_with_same_name_exist()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            Assert.Throws<InvalidOperationException>(() => typeResolver.Resolve(nameof(Planet)));
        }

        [IntegrationTest]
        public void It_can_be_overriden()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var resolveMethod = typeResolver.GetType().GetMethod(nameof(TypeResolver.Resolve));

            Assert.True(resolveMethod.IsVirtual);
        }
    }

    [Collection(nameof(RepositoriesCollection))]
    // ReSharper disable once InconsistentNaming the literal name of the vendor.
    public class SQLiteTypeResolverTests : TypeResolverTests<SQLiteRepositoryFixture>
    {
        public SQLiteTypeResolverTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }

        /// <summary>Dulpicate named class</summary>
        public class Planet : Entity { }
    }

    [Collection(RepositoriesMsSqlCollection.Name)]
    public class MsSqlTypeResolverTests : TypeResolverTests<MsSqlRepositoryFixture>
    {
        public MsSqlTypeResolverTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesOracleCollection.Name)]
    public class OracleTypeResolverTests : TypeResolverTests<OracleRepositoryFixture>
    {
        public OracleTypeResolverTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
