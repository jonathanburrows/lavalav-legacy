using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Repositories.Tests.Configuration;
using lvl.Repositories.Tests.Fixtures;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;

namespace lvl.Repositories.Tests
{
    public abstract class RepositoryFactoryTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture
    {
        private IServiceProvider Services { get; }

        protected RepositoryFactoryTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryRepositoriesFixture));
        }

        [IntegrationTest]
        public void Generic_construct_will_create_repository_for_entity_type()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct<Moon>();
            Assert.NotNull(repository);
        }

        [IntegrationTest]
        public void Construct_will_create_repository_for_entity_typw()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct(typeof(Moon));
            Assert.IsAssignableFrom<IRepository<Moon>>(repository);
        }

        [IntegrationTest]
        public void Construct_will_throw_argument_null_exception_when_type_is_null()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentNullException>(() => repositoryFactory.Construct(null));
        }

        [IntegrationTest]
        public void Construct_will_throw_argument_exception_when_type_isntly_exception()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentException>(() => repositoryFactory.Construct(typeof(NonEntity)));
        }

        [IntegrationTest]
        public void Construct_generically_will_throw_argument_exception_when_entity_type_isnt_mapped()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [IntegrationTest]
        public void Construct_will_throw_target_invocation_exception_when_entity_type_isnt_mapped()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.Throws<TargetInvocationException>(() => repositoryFactory.Construct(typeof(UnmappedEntity)));
        }

        private class UnmappedEntity : Entity, IAggregateRoot { }

        private class NonEntity { }
    }

    [Collection(nameof(RepositoriesCollection))]
    // ReSharper disable once InconsistentNaming Matches the vendor name literally.
    public class SQLiteRepositoryFactoryTests : RepositoryFactoryTests<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryFactoryTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesMsSqlCollection.Name)]
    public class MsSqlRepositoryFactoryTests : RepositoryFactoryTests<MsSqlRepositoryFixture>
    {
        public MsSqlRepositoryFactoryTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesOracleCollection.Name)]
    public class OracleRepositoryFactoryTests : RepositoryFactoryTests<OracleRepositoryFixture>
    {
        public OracleRepositoryFactoryTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
