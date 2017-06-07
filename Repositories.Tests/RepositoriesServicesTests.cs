using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using lvl.TestDomain;
using lvl.Ontology;
using lvl.Repositories.Tests.Fixtures;
using lvl.Repositories.Tests.Configuration;
using lvl.Repositories.Authorization;

namespace lvl.Repositories.Tests
{
    /// <remarks>
    ///     Shortened class name as it was becoming too unweildy
    /// </remarks>
    public abstract class RepositoriesServicesTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture 
    {
        protected IServiceProvider Services { get; }

        protected RepositoriesServicesTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryRepositoriesFixture));
        }

        [IntegrationTest]
        public void It_will_allow_type_resolved_to_be_resolved()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();

            Assert.NotNull(typeResolver);
        }

        [IntegrationTest]
        public void It_will_allow_repository_factory_to_be_resolved()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.NotNull(repositoryFactory);
        }

        [IntegrationTest]
        public void It_will_allow_aggregate_root_filter_to_be_resolved()
        {
            var aggregateRootFilter = Services.GetRequiredService<AggregateRootFilter>();

            Assert.NotNull(aggregateRootFilter);
        }

        [IntegrationTest]
        public void It_will_allow_generic_repository_to_be_resolved_with_a_concrete_type()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            Assert.NotNull(repository);
        }

        [IntegrationTest]
        public void It_will_throw_invalid_operation_exception_when_resolving_repository_for_unmapped_type()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [IntegrationTest]
        public void It_will_allow_session_provider_to_be_resolved()
        {
            var sessionFactory = Services.GetRequiredService<SessionProvider>();

            Assert.NotNull(sessionFactory);
        }

        [IntegrationTest]
        public void It_will_allow_generic_repository_to_be_overridden()
        {
            var mockedServices = new ServiceCollection()
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddScoped<IRepository<Moon>, MockRepository<Moon>>()
                .BuildServiceProvider();

            var repository = mockedServices.GetRequiredService<IRepository<Moon>>();

            Assert.IsType<MockRepository<Moon>>(repository);
        }

        [IntegrationTest]
        public void It_will_return_overriden_repository_when_resolving_with_repository_factory()
        {
            var mockedServices = new ServiceCollection()
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddScoped<IRepository<Moon>, MockRepository<Moon>>()
                .BuildServiceProvider();
            var repositoryFactory = mockedServices.GetRequiredService<RepositoryFactory>();

            var repository = repositoryFactory.Construct<Moon>();

            Assert.IsType<MockRepository<Moon>>(repository);
        }

        [IntegrationTest]
        public void It_will_throw_invalid_operation_exception_when_adding_repositories_without_calling_add_domains()
        {
            var services = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => services.AddRepositories());
        }

        // ReSharper disable once ClassNeverInstantiated.Local Used by reflection
        private class UnmappedEntity : Entity, IAggregateRoot { }

        // ReSharper disable once ClassNeverInstantiated.Local Used by reflection
        private class MockRepository<TEntity> : Repository<TEntity> where TEntity : Entity, IAggregateRoot
        {
            public MockRepository(SessionProvider sessionManager, AggregateRootFilter aggregateRootFilter) : base(sessionManager, aggregateRootFilter) { }
        }
    }

    [Collection(nameof(RepositoriesCollection))]
    // ReSharper disable once InconsistentNaming matches the vendor name literally.
    public class SQLiteRepositoryServicesTests : RepositoriesServicesTests<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryServicesTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }


        [Fact]
        public void AfterAddingRepository_WhenUsingSQLite_PersistentSessionProviderResolved()
        {
            var sessionFactory = Services.GetRequiredService<SessionProvider>();

            Assert.IsNotType<SessionProvider>(sessionFactory);
        }
    }

    [Collection(RepositoriesMsSqlCollection.Name)]
    public class MsSqlRepositoryServicesTests : RepositoriesServicesTests<MsSqlRepositoryFixture>
    {
        public MsSqlRepositoryServicesTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesOracleCollection.Name)]
    public class OracleRepositoryServicesTests : RepositoriesServicesTests<OracleRepositoryFixture>
    {
        public OracleRepositoryServicesTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
