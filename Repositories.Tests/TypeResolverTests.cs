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
        public void WhenResolvingName_AndGivenFullyQualifiedEntity_TypeIsResolved()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var resolvedType = typeResolver.Resolve(moonType.FullName);

            Assert.Equal(resolvedType, moonType);
        }

        [IntegrationTest]
        public void WhenResolvingName_AndGivenFullyQualifiedEntityWithWrongCase_TypeIsResolved()
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
        public void WhenResolvingName_AndGivenClassNameOfEntity_TypeIsResolved()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var moonType = typeof(Moon);
            var resolvedType = typeResolver.Resolve(moonType.Name);

            Assert.Equal(resolvedType, moonType);
        }

        [IntegrationTest]
        public void WhenResolvingName_AndGivenClassNameOfEntityWithWrongCase_TypeIsResolved()
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
        public void WhenResolvingName_AndNameIsNull_ArgumentNullExceptionIsThrown()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            Assert.Throws<ArgumentNullException>(() => typeResolver.Resolve(null));
        }

        [IntegrationTest]
        public void WhenResolvingName_AndTwoExist_InvalidOperationIsThrown()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            Assert.Throws<InvalidOperationException>(() => typeResolver.Resolve(nameof(Planet)));
        }

        [IntegrationTest]
        public void ResolveMethodIsOverridable()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();
            var resolveMethod = typeResolver.GetType().GetMethod(nameof(TypeResolver.Resolve));

            Assert.True(resolveMethod.IsVirtual);
        }
    }

    [Collection(RepositoriesCollection.Name)]
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
