using System;
using Xunit;

namespace lvl.Repositories.Tests
{
    public class RepositoryFactoryTests : IClassFixture<InMemoryRepositoriesFixture>
    {
        private IServiceProvider Services { get; }

        public RepositoryFactoryTests(InMemoryRepositoriesFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture.ServiceProvider;
        }

        [Fact]
        public void WhenConstructingGenerically_CreatesRepositoryForEntityType()
        {
            Assert.True(false);
        }

        [Fact]
        public void WhenConstructing_CreatesRepositoryForEntityType()
        {
            Assert.True(false);
        }

        [Fact]
        public void WhenConstructing_AndNullTypeGiven_ArgumentNullIsThrown()
        {
            Assert.True(false);
        }

        [Fact]
        public void WhenConstructing_AndTypeIsntEntity_ArgumentExceptionIsThrown()
        {
            Assert.True(false);
        }

        [Fact]
        public void WhenConstructingGenerically_AndEntityTypeIsntMapped_ArgumentExceptionIsThrow()
        {
            Assert.True(false);
        }

        [Fact]
        public void WhenConstructing_AndEntityTypeIsNotMapped_ArgumentExceptionIsThrown() {
            Assert.True(false);
        }
    }
}
