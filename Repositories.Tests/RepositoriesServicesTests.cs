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
    /// Shortened class name as it was becoming too unweildy
    /// </remarks>
    public abstract class RepositoriesServicesTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture 
    {
        protected IServiceProvider Services { get; }

        protected RepositoriesServicesTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryRepositoriesFixture));
        }

        [IntegrationTest]
        public void AfterAddingRepositories_WhenResolvingTypeResolver_ValueIsReturned()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();

            Assert.NotNull(typeResolver);
        }

        [IntegrationTest]
        public void AfterAddingRepositories_WhenResolvingRepositoryFactory_ValueIsReturned()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.NotNull(repositoryFactory);
        }

        [IntegrationTest]
        public void AfterAddingRepositories_WhenResolvingAggregateRootFilter_ValueIsReturned()
        {
            var aggregateRootFilter = Services.GetRequiredService<AggregateRootFilter>();

            Assert.NotNull(aggregateRootFilter);
        }

        [IntegrationTest]
        public void AfterAddingRepositories_WhenResolvingRepositoryForMappedType_ValueIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            Assert.NotNull(repository);
        }

        [IntegrationTest]
        public void AfteringAddingRepository_WhenResolvingRepositoryForUnmappedType_InvalidOperationIsThrown()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [IntegrationTest]
        public void AfterAddingRepository_WhenResolvingSessionProvider_ValueIsReturned()
        {
            var sessionFactory = Services.GetRequiredService<SessionProvider>();

            Assert.NotNull(sessionFactory);
        }

        [IntegrationTest]
        public void AfterOverridingRepository_WhenResolvingThroughServiceProvider_OverriddenTypeIsReturned()
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
        public void AfterOverridingRepository_WhenResolvingThroughFactory_OverridenTypeIsReturned()
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
        public void WhenAddingRepositories_WithoutCallingAddDomains_ThrowsInvalidOperationException()
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

    [Collection(RepositoriesCollection.Name)]
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
