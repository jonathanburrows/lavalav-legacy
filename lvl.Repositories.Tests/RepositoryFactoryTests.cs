using lvl.Ontology;
using lvl.Repositories.Tests.Fixtures;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;

namespace lvl.Repositories.Tests
{
    public abstract class RepositoryFactoryTests<TRepositoryFixture> : IClassFixture<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture
    {
        private IServiceProvider Services { get; }

        public RepositoryFactoryTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture.ServiceProvider;
        }

        [Fact]
        public void WhenConstructingGenerically_CreatesRepositoryForEntityType()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct<Moon>();
            Assert.NotNull(repository);
        }

        [Fact]
        public void WhenConstructing_CreatesRepositoryForEntityType()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct(typeof(Moon));
            Assert.IsAssignableFrom<IRepository<Moon>>(repository);
        }

        [Fact]
        public void WhenConstructing_AndNullTypeGiven_ArgumentNullIsThrown()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentNullException>(() => repositoryFactory.Construct(null));
        }

        [Fact]
        public void WhenConstructing_AndTypeIsntEntity_ArgumentExceptionIsThrown()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentException>(() => repositoryFactory.Construct(typeof(NonEntity)));
        }

        [Fact]
        public void WhenConstructingGenerically_AndEntityTypeIsntMapped_ArgumentExceptionIsThrow()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [Fact]
        public void WhenConstructing_AndEntityTypeIsNotMapped_TargetInvocationExceptionIsThrown()
            {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();

            Assert.Throws<TargetInvocationException>(() => repositoryFactory.Construct(typeof(UnmappedEntity)));
        }

        private class UnmappedEntity : IEntity
        {
            public int Id { get; set; }
        }

        private class NonEntity { }
    }

    [Collection(nameof(SQLiteRepositoryFactoryTests))]
    public class SQLiteRepositoryFactoryTests : RepositoryFactoryTests<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryFactoryTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(nameof(MsSqlRepositoryFactoryTests))]
    public class MsSqlRepositoryFactoryTests : RepositoryFactoryTests<MsSqlRepositoryFixture>
    {
        public MsSqlRepositoryFactoryTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(nameof(OracleRepositoryFactoryTests))]
    internal class OracleRepositoryFactoryTests : RepositoryFactoryTests<OracleRepositoryFixture>
    {
        public OracleRepositoryFactoryTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
