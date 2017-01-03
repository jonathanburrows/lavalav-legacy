using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using lvl.TestDomain;
using lvl.Ontology;
using lvl.Repositories.Tests.Fixtures;

namespace lvl.Repositories.Tests
{
    /// <remarks>
    /// Shortened class name as it was becoming too unweildy
    /// </remarks>
    public abstract class RepositoriesServicesTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture
    {
        protected IServiceProvider Services { get; }

        public RepositoriesServicesTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture.ServiceProvider;
        }

        [Fact]
        public void AfterAddingRepositories_WhenResolvingTypeResolver_ValueIsReturned()
        {
            var typeResolver = Services.GetRequiredService<TypeResolver>();

            Assert.NotNull(typeResolver);
        }

        [Fact]
        public void AfterAddingRepositories_WhenResolvingRepositoryFactory_ValueIsReturned()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.NotNull(repositoryFactory);
        }

        [Fact]
        public void AfterAddingRepositories_WhenResolvingRepositoryForMappedType_ValueIsReturned()
        {
            var mappedType = typeof(Moon);

            var repository = Services.GetRequiredService<IRepository<Moon>>();

            Assert.NotNull(repository);
        }

        [Fact]
        public void AfteringAddingRepository_WhenResolvingRepositoryForUnmappedType_InvalidOperationIsThrown()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [Fact]
        public void AfterAddingRepository_WhenResolvingSessionProvider_ValueIsReturned()
        {
            var sessionFactory = Services.GetRequiredService<SessionProvider>();

            Assert.NotNull(sessionFactory);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void WhenAddingRepositories_WithoutCallingAddDomains_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => services.AddRepositories());
        }

        private class UnmappedEntity : IEntity
        {
            public int Id { get; set; }
        }

        private class MockRepository<TEntity> : Repository<TEntity> where TEntity : class, IEntity
        {
            public MockRepository(SessionProvider sessionManager) : base(sessionManager) { }
        }
    }

    [Collection(RepositoriesCollection.Name)]
    public class SQLiteRepositoryServicesTests : RepositoriesServicesTests<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryServicesTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }


        [Fact]
        public void AfterAddingRepository_WhenUsingSQLite_PersistentSessionProviderResolved()
        {
            var sessionFactory = Services.GetRequiredService<SessionProvider>();

            Assert.IsAssignableFrom<SQLitePersistentSessionProvider>(sessionFactory);
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
