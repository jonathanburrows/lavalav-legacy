using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Repositories.Tests
{
    public abstract class RepositoryTests
    {
        private IServiceProvider Services { get; }

        public RepositoryTests(RepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture.ServiceProvider;
        }

        [Fact]
        public async Task GetCollection_OnRepositoryWithMultipleElements_ReturnsMultipleElement()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon { });
            await repository.CreateAsync(new Moon { });

            var entities = await repository.GetAsync();

            Assert.True(entities.Count() > 0);
        }

        [Fact]
        public async Task GetSingle_OnRepositoryWithMatchingElement_ReturnsElement()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon { });

            var fetched = await repository.GetAsync(fetching.Id);
            Assert.NotNull(fetched);
        }

        [Fact]
        public async Task GetSingle_OnRepositoryWithNoMatchingElement_ReturnsNull()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var fetched = await repository.GetAsync(0);

            Assert.Null(fetched);
        }

        [Fact]
        public async Task GenericCreate_IncreasesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon { });
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore + 1, countAfter);
        }

        [Fact]
        public async Task Create_IncreasesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon { });

            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countAfter, countBefore + 1);
        }

        [Fact]
        public async Task GenericCreate_PopulatesId()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var created = await repository.CreateAsync(new Moon { });

            Assert.True(created.Id > 0);
        }

        [Fact]
        public async Task Create_PopulatesId()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var creating = new Moon { };

            await repository.CreateAsync(creating);

            Assert.True(creating.Id > 0);
        }

        [Fact]
        public async Task GenericCreate_WhenAddingNull_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.CreateAsync(null));
        }

        [Fact]
        public async Task Create_WhenAddingNull_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            try
            {
                await repository.CreateAsync(null);
                throw new Exception($"No exception was thrown, was expecting {nameof(ArgumentNullException)}");
            }
            catch (Exception e)
            {
                if (!(e is ArgumentNullException) && !(e.InnerException is ArgumentNullException))
                {
                    throw e;
                }
            }
        }

        [Fact]
        public async Task Create_WhenNotOfCorrectType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentException>(async () => await repository.CreateAsync(new Planet { }));
        }

        [Fact]
        public async Task WhenCreating_AndHasNewChildEntity_ChildEntityCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var newChildEntity = new Planet { };
            var creating = new Moon { Planet = newChildEntity };

            await repository.CreateAsync(creating);

            Assert.True(newChildEntity.Id > 0);
        }

        [Fact]
        public async Task WhenCreating_AndHasNewEntityInChildCollection_ChildEntityCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var newChildEntity = new Moon { };
            var creating = new Planet
            {
                Moons = new[] { newChildEntity }
            };

            await repository.CreateAsync(creating);

            Assert.True(newChildEntity.Id > 0);
        }

        [Fact]
        public async Task WhenCreating_AndHasEditedChildEntity_ChildEntityIsUpdated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var existingChildEntity = new Planet { Name = "Earth" };
            await planetRepository.CreateAsync(existingChildEntity);
            var creating = new Moon { Planet = existingChildEntity };

            existingChildEntity.Name = "Terra";
            await moonRepository.CreateAsync(creating);

            var updatedChild = await planetRepository.GetAsync(existingChildEntity.Id);
            Assert.Equal(existingChildEntity.Name, updatedChild.Name);
        }

        [Fact]
        public async Task WhenCreating_AndHasEditedEntityInChildCollection_ChildEntityIsUpdated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var existingChildEntity = new Moon { Name = "Old Moon" };
            await moonRepository.CreateAsync(existingChildEntity);
            var creating = new Planet
            {
                Moons = new[] { existingChildEntity }
            };

            existingChildEntity.Name = "New Moon";
            await planetRepository.CreateAsync(creating);

            var updatedChild = await moonRepository.GetAsync(existingChildEntity.Id);
            Assert.Equal(existingChildEntity.Name, updatedChild.Name);
        }

        [Fact]
        public async Task GenericUpdate_WhenSuccessful_UpdatesPropertiesOfMatchingEntity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var original = await repository.CreateAsync(new Moon { Name = "Old Moon" });

            var updating = new Moon { Id = original.Id, Name = "New Moon" };
            await repository.UpdateAsync(updating);

            var updated = await repository.GetAsync(original.Id);
            Assert.Equal(updated.Name, updating.Name);
        }

        [Fact]
        public async Task Update_WhenSuccessful_UpdatesPropertiesOfMatchingEntity()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var original = new Moon { Name = "Old Moon" };
            await repository.CreateAsync(original);

            var updating = new Moon { Id = original.Id, Name = "New Moon" };
            await repository.UpdateAsync(updating);

            var updated = (Moon)await repository.GetAsync(original.Id);
            Assert.Equal(updated.Name, updating.Name);
        }

        [Fact]
        public async Task GenericUpdate_WhenGivenNullValue_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.UpdateAsync(null));
        }

        [Fact]
        public async Task Update_WhenGivenNullValue_ThrowsArgumentNullException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.UpdateAsync(null));
        }

        [Fact]
        public async Task Update_WhenEntityTypeIsntRepositoryType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.UpdateAsync(new Planet { }));
        }

        [Fact]
        public async Task GenericUpdate_WhenNoMatchingEntity_ThrowsInvalidOperationException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.UpdateAsync(new Moon { }));
        }

        [Fact]
        public async Task Update_WhenNoMatchingEntity_ThrowsInvalidOperationException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.UpdateAsync(new Moon { }));
        }

        [Fact]
        public async Task WhenUpdating_AndHasNewChildEntity_ChildIsCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var updating = await repository.CreateAsync(new Moon { });
            updating.Planet = new Planet { };

            await repository.UpdateAsync(updating);

            Assert.True(updating.Planet.Id > 0);
        }

        [Fact]
        public async Task WhenUpdating_AndHasNewEntityInChildCollection_ChildIsCreated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var updating = await planetRepository.CreateAsync(new Planet { });

            var newChild = new Moon { };
            updating.Moons = new[] { newChild };
            await planetRepository.UpdateAsync(updating);

            Assert.True(newChild.Id > 0);
        }

        [Fact]
        public async Task WhenUpdating_AndHasEditedChildEntity_ChildIsUpdated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var existingChild = new Planet { Name = "Earth" };
            var updating = new Moon { Planet = existingChild };
            await moonRepository.CreateAsync(updating);

            existingChild.Name = "Terra";
            await moonRepository.UpdateAsync(updating);

            var updatedChild = await planetRepository.GetAsync(existingChild.Id);
            Assert.Equal(existingChild.Name, updatedChild.Name);
        }

        [Fact]
        public async Task WhenUpdating_AndHasEditedEntityInChildCollection_ChildIsUpdated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var existingChild = new Moon { Name = "Old Moon" };
            var updating = new Planet
            {
                Moons = new[] { existingChild }
            };
            await planetRepository.CreateAsync(updating);

            existingChild.Name = "New Moon";
            await planetRepository.UpdateAsync(updating);

            var updatedChild = await moonRepository.GetAsync(existingChild.Id);
            Assert.Equal(existingChild.Name, updatedChild.Name);
        }

        [Fact]
        public async Task GenericDelete_WhenSuccessful_ReducesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon { });
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_ReducesSizeByOne()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon { });
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [Fact]
        public async Task GenericDelete_WhenSuccessful_MakesGettingEntityReturnNull()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = new Moon { };
            await repository.CreateAsync(deleting);

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_MakesGettingEntityReturnNull()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon { });

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task GenericDelete_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.DeleteAsync(null));
        }

        [Fact]
        public async Task Delete_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.DeleteAsync(null));
        }

        [Fact]
        public async Task Delete_WhenEntityTypeIsntRepositoryType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var planet = new Planet { };

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.DeleteAsync(planet));
        }

        [Fact]
        public async Task GenericDelete_WhenEntityHasNoMatchingElement_ThrowsInvalidOperationException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.DeleteAsync(new Moon()));
        }

        [Fact]
        public async Task Delete_WhenEntityHasNoMatchingElement_ThrowsInvalidOperationException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var unmatchedElement = new Moon { Id = int.MaxValue };

            await Assert.ThrowsAnyAsync<Exception>(async () => await repository.DeleteAsync(unmatchedElement));
        }

        [Fact]
        public async Task WhenDeleting_WhenReferencingChild_ChildStillExists()
        {
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var deleting = new Moon { Planet = new Planet { } };
            await moonRepository.CreateAsync(deleting);

            await moonRepository.DeleteAsync(deleting);

            var child = planetRepository.GetAsync(deleting.Planet.Id);
            Assert.NotNull(child);
        }

        [Fact]
        public async Task WhenDeleting_WhenReferencingChildCollection_ChildrenAreRemoved()
        {
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var child = new Moon { };
            var parent = new Planet
            {
                Moons = new[] { child }
            };
            await planetRepository.CreateAsync(parent);

            await planetRepository.DeleteAsync(parent);

            var deletedChild = await planetRepository.GetAsync(child.Id);
            Assert.Null(deletedChild);
        }
    }

    [Collection(nameof(SQLiteRepositoryTests))]
    public class SQLiteRepositoryTests : RepositoryTests, IClassFixture<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    /// <remarks>To disable, make internal</remarks>
    [Collection(nameof(MsSqlRepositoryTests))]
    public class MsSqlRepositoryTests : RepositoryTests, IClassFixture<MsSqlRepositoryFixture>
    {
        public MsSqlRepositoryTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    /// <remarks>To disable, make internal</remarks>
    [Collection(nameof(OracleRepositoryTests))]
    public class OracleRepositoryTests : RepositoryTests, IClassFixture<OracleRepositoryFixture>
    {
        public OracleRepositoryTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
