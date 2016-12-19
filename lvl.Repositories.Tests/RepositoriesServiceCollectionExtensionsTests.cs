using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using lvl.TestDomain;

namespace lvl.Repositories.Tests
{
    public class RepositoriesServiceCollectionExtensionsTests : IClassFixture<InMemoryRepositoriesFixture>
    {
        private IServiceProvider Services { get; }

        public RepositoriesServiceCollectionExtensionsTests(InMemoryRepositoriesFixture inMemoryRepositoriesFixture)
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
            Assert.False(true);
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
            Assert.False(true);
        }
    }
}
