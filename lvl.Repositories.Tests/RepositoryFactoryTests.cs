﻿using lvl.Ontology;
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

        public RepositoryFactoryTests(TRepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture.ServiceProvider;
        }

        [IntegrationTest]
        public void WhenConstructingGenerically_CreatesRepositoryForEntityType()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct<Moon>();
            Assert.NotNull(repository);
        }

        [IntegrationTest]
        public void WhenConstructing_CreatesRepositoryForEntityType()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            var repository = repositoryFactory.Construct(typeof(Moon));
            Assert.IsAssignableFrom<IRepository<Moon>>(repository);
        }

        [IntegrationTest]
        public void WhenConstructing_AndNullTypeGiven_ArgumentNullIsThrown()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentNullException>(() => repositoryFactory.Construct(null));
        }

        [IntegrationTest]
        public void WhenConstructing_AndTypeIsntEntity_ArgumentExceptionIsThrown()
        { 
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<ArgumentException>(() => repositoryFactory.Construct(typeof(NonEntity)));
        }

        [IntegrationTest]
        public void WhenConstructingGenerically_AndEntityTypeIsntMapped_ArgumentExceptionIsThrow()
        {
            var repositoryFactory = Services.GetRequiredService<RepositoryFactory>();
            Assert.Throws<InvalidOperationException>(() => repositoryFactory.Construct<UnmappedEntity>());
        }

        [IntegrationTest]
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

    [Collection(RepositoriesCollection.Name)]
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
