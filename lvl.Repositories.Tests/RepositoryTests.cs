using lvl.Repositories.Querying;
using lvl.Repositories.Tests.Configuration;
using lvl.Repositories.Tests.Fixtures;
using lvl.TestDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Repositories.Tests
{
    // ReSharper disable All The use of generics here is important for test re-use.
    public abstract class RepositoryTests<TRepositoryFixture> where TRepositoryFixture : RepositoryFixture
    {
        private IServiceProvider Services { get; }

        protected RepositoryTests(RepositoryFixture inMemoryRepositoriesFixture)
        {
            Services = inMemoryRepositoriesFixture?.ServiceProvider ?? throw new ArgumentNullException(nameof(inMemoryRepositoriesFixture));
        }

        [IntegrationTest]
        public async Task GetCollection_OnRepositoryWithMultipleElements_ReturnsMultipleElement()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());

            var entities = await repository.GetAsync();

            Assert.True(entities.Any());
        }

        [IntegrationTest]
        public async Task GetSingle_OnRepositoryWithMatchingElement_ReturnsElement()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon());

            var fetched = await repository.GetAsync(fetching.Id);
            Assert.NotNull(fetched);
        }

        [IntegrationTest]
        public async Task GetSingle_OnRepositoryWithNoMatchingElement_ReturnsNull()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var fetched = await repository.GetAsync(0);

            Assert.Null(fetched);
        }

        [IntegrationTest]
        public async Task GenericCreate_IncreasesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon());
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore + 1, countAfter);
        }

        [IntegrationTest]
        public async Task Create_IncreasesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon());

            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countAfter, countBefore + 1);
        }

        [IntegrationTest]
        public async Task GenericCreate_PopulatesId()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var created = await repository.CreateAsync(new Moon());

            Assert.True(created.Id > 0);
        }

        [IntegrationTest]
        public async Task Create_PopulatesId()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var creating = new Moon();

            await repository.CreateAsync(creating);

            Assert.True(creating.Id > 0);
        }

        [IntegrationTest]
        public async Task GenericCreate_WhenAddingNull_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.CreateAsync(null));
        }

        [IntegrationTest]
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
                    throw;
                }
            }
        }

        [IntegrationTest]
        public async Task Create_WhenNotOfCorrectType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentException>(() => repository.CreateAsync(new Planet()));
        }

        [IntegrationTest]
        public async Task WhenGenericallyCreating_EntityWithIdentifier_InvalidOperationExceptionIsThrown()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var entityWithIdentifier = new Moon { Id = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.CreateAsync(entityWithIdentifier));
        }

        [IntegrationTest]
        public async Task WhenCreating_AndHasNewChildEntity_ChildEntityCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var newChildEntity = new Planet();
            var creating = new Moon { Planet = newChildEntity };

            await repository.CreateAsync(creating);

            Assert.True(newChildEntity.Id > 0);
        }

        [IntegrationTest]
        public async Task WhenCreating_AndHasNewEntityInChildCollection_ChildEntityCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var newChildEntity = new Moon();
            var creating = new Planet
            {
                Moons = new[] { newChildEntity }
            };

            await repository.CreateAsync(creating);

            Assert.True(newChildEntity.Id > 0);
        }

        [IntegrationTest]
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

        [IntegrationTest]
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

        [IntegrationTest]
        public async Task GenericUpdate_WhenSuccessful_UpdatesPropertiesOfMatchingEntity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var original = await repository.CreateAsync(new Moon { Name = "Old Moon" });

            var updating = new Moon { Id = original.Id, Name = "New Moon" };
            await repository.UpdateAsync(updating);

            var updated = await repository.GetAsync(original.Id);
            Assert.Equal(updated.Name, updating.Name);
        }

        [IntegrationTest]
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

        [IntegrationTest]
        public async Task GenericUpdate_WhenGivenNullValue_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(null));
        }

        [IntegrationTest]
        public async Task Update_WhenGivenNullValue_ThrowsArgumentNullException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(null));
        }

        [IntegrationTest]
        public async Task Update_WhenEntityTypeIsntRepositoryType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(new Planet()));
        }

        [IntegrationTest]
        public async Task GenericUpdate_WhenNoMatchingEntity_ThrowsInvalidOperationException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task Update_WhenNoMatchingEntity_ThrowsInvalidOperationException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task WhenUpdating_AndHasNewChildEntity_ChildIsCreated()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var updating = await repository.CreateAsync(new Moon());
            updating.Planet = new Planet();

            await repository.UpdateAsync(updating);

            Assert.True(updating.Planet.Id > 0);
        }

        [IntegrationTest]
        public async Task WhenUpdating_AndHasNewEntityInChildCollection_ChildIsCreated()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var updating = await planetRepository.CreateAsync(new Planet());

            var newChild = new Moon();
            updating.Moons = new[] { newChild };
            await planetRepository.UpdateAsync(updating);

            Assert.True(newChild.Id > 0);
        }

        [IntegrationTest]
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

        [IntegrationTest]
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

        [IntegrationTest]
        public async Task GenericDelete_WhenSuccessful_ReducesSizeByOne()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [IntegrationTest]
        public async Task Delete_WhenSuccessful_ReducesSizeByOne()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [IntegrationTest]
        public async Task GenericDelete_WhenSuccessful_MakesGettingEntityReturnNull()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = new Moon();
            await repository.CreateAsync(deleting);

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [IntegrationTest]
        public async Task Delete_WhenSuccessful_MakesGettingEntityReturnNull()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [IntegrationTest]
        public async Task GenericDelete_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(null));
        }

        [IntegrationTest]
        public async Task Delete_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(null));
        }

        [IntegrationTest]
        public async Task Delete_WhenEntityTypeIsntRepositoryType_ThrowsArgumentException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var planet = new Planet();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(planet));
        }

        [IntegrationTest]
        public async Task GenericDelete_WhenEntityHasNoMatchingElement_ThrowsInvalidOperationException()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task Delete_WhenEntityHasNoMatchingElement_ThrowsInvalidOperationException()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var unmatchedElement = new Moon { Id = int.MaxValue };

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(unmatchedElement));
        }

        [IntegrationTest]
        public async Task WhenDeleting_WhenReferencingChild_ChildStillExists()
        {
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var deleting = new Moon { Planet = new Planet()};
            await moonRepository.CreateAsync(deleting);

            await moonRepository.DeleteAsync(deleting);

            var child = planetRepository.GetAsync(deleting.Planet.Id);
            Assert.NotNull(child);
        }

        [IntegrationTest]
        public async Task WhenDeleting_WhenReferencingChildCollection_ChildrenAreRemoved()
        {
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var child = new Moon();
            var parent = new Planet
            {
                Moons = new[] { child }
            };
            await planetRepository.CreateAsync(parent);

            await planetRepository.DeleteAsync(parent);

            var deletedChild = await moonRepository.GetAsync(child.Id);
            Assert.Null(deletedChild);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndApplingFilter_MatchingEntitiesAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Where(planet => planet.SupportsLife);

            var queryResults = await repository.GetAsync(query);

            Assert.True(queryResults.Items.All(p => p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndApplyingFilter_UnmatchedEntitiesAreNotReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Where(planet => planet.SupportsLife);

            var queryResults = await repository.GetAsync(query);

            Assert.False(queryResults.Items.Any(p => !p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndApplingDynamicFilter_MatchingEntitiesAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Where($"{nameof(Planet.SupportsLife)}={true}");

            var queryResults = await repository.GetAsync(query);

            Assert.True(queryResults.Items.All(p => p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndApplingDynamicEqualsFilter_MatchingEntitiesAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = "Hello", SupportsLife = true });
            await repository.CreateAsync(new Planet { Name = "World", SupportsLife = true });
            await repository.CreateAsync(new Planet { Name = "true", SupportsLife = false });
            var query = new Query<Planet>().Where($@"{nameof(Planet.Name)}.ToLower() = ""hello""");

            var queryResults = await repository.GetAsync(query);

            Assert.True(queryResults.Items.All(p => p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndApplyingDynamicFilter_UnmatchedEntitiesAreNotReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Where($"{nameof(Planet.SupportsLife)}={true}");

            var queryResults = await repository.GetAsync(query);

            Assert.False(queryResults.Items.Any(p => !p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQueryingAndTakingRecords_AndTheresMoreRecordsThanTheTakeAmount_OnlyTakeAmountIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Where($"{nameof(Planet.SupportsLife)}={true}");

            var queryResults = await repository.GetAsync(query);

            Assert.False(queryResults.Items.Any(p => !p.SupportsLife));
        }

        [IntegrationTest]
        public async Task WhenQueryingAndSkipping_SkippingRecordsArentReturned()
        {
            var skip = 2;
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var total = (await repository.GetAsync()).Count();
            var query = new Query<Planet>().Skip(skip);

            var queryResults = await repository.GetAsync(query);

            Assert.Equal(total - skip, queryResults.Items.Count());
        }

        [IntegrationTest]
        public async Task WhenQueryingAndTaking_TakenRecordsAreReturned()
        {
            var take = 2;
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = new Query<Planet>().Take(take);

            var queryResults = await repository.GetAsync(query);

            Assert.Equal(take, queryResults.Items.Count());
        }

        [IntegrationTest]
        public async Task WhenQuery_AndApplyingFilter_MatchedCountIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var query = new Query<Planet>().Where(planet => planet.SupportsLife);
            var countBefore = (await repository.GetAsync(query)).Count;
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });

            var queryResults = await repository.GetAsync(query);

            Assert.Equal(2, queryResults.Count - countBefore);
        }

        [IntegrationTest]
        public async Task WhenQuery_AndApplyingDynamicFilter_MatchedCountIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var query = new Query<Planet>().Where($"{nameof(Planet.SupportsLife)}={true}");
            var countBefore = (await repository.GetAsync(query)).Count;
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = true });
            await repository.CreateAsync(new Planet { SupportsLife = false });

            var queryResults = await repository.GetAsync(query);

            Assert.Equal(2, queryResults.Count - countBefore);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndTaking_CountIncludesRecordsNotInResult()
        {
            var take = 2;
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            await repository.CreateAsync(new Planet());
            await repository.CreateAsync(new Planet());
            var query = new Query<Planet>().Take(take);

            var queryResult = await repository.GetAsync(query);

            Assert.True(queryResult.Count > take);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndSkipping_CountIncludesRecordsNotInResult()
        {
            var skip = 2;
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            await repository.CreateAsync(new Planet());
            await repository.CreateAsync(new Planet());
            var query = new Query<Planet>().Skip(skip);

            var queryResult = await repository.GetAsync(query);

            Assert.True(queryResult.Count > skip);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndSelectingProperty_ArrayOfThatPropertyIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = new Query<Planet>().Select(planet => planet.Name);

            var queryResult = await repository.GetAsync(query);

            Assert.Contains("Earth", queryResult.Items);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndSelectingAnonymousObject_AnonymousObjectsArePopulatedAndReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var earth = await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = new Query<Planet>().Select(planet => new
            {
                planet.Id,
                EnglishName = planet.Name
            });

            var queryResult = await repository.GetAsync(query);
            var selectedEarth = queryResult.Items.Single(p => p.Id == earth.Id);

            Assert.Equal("Earth", selectedEarth.EnglishName);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndSelectingChildProperty_ChildPropertyIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var lunar = new Moon { Planet = new Planet { Name = "Terra" } };
            await repository.CreateAsync(lunar);
            var query = new Query<Moon>().Select(moon => moon.Planet);

            var queryResult = await repository.GetAsync(query);
            var queriedPlanet = queryResult.Items.Single(p => p?.Id == lunar.Planet.Id);

            Assert.Equal("Terra", queriedPlanet.Name);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndOrderingByName_CorrectlySortsResult()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A" },
                new Planet { Name = "C" },
                new Planet { Name = "B" }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderBy(p => p.Name);

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderBy(p => p.Name);
            var inOrder = linqOrdered.Zip(queryResult.Items, (a, b) => new { a, b }).All(z => z.a.Name == z.b.Name);

            Assert.True(inOrder);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndDynamicallyOrderingByName_CorrectlySortsResult()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A" },
                new Planet { Name = "C" },
                new Planet { Name = "B" }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderBy(nameof(Planet.Name));

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderBy(p => p.Name);
            var inOrder = queryResult.Items.SequenceEqual(linqOrdered);

            Assert.True(inOrder);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndOrderingByNameDescending_ResultsAreCorrectlySorted()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A" },
                new Planet { Name = "C" },
                new Planet { Name = "B" }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderByDescending(p => p.Name);

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderByDescending(p => p.Name);
            var inOrder = queryResult.Items.SequenceEqual(linqOrdered);

            Assert.True(inOrder);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndOrderDynamicallyByNameDescending_ResultsAreCorrectlySorted()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A" },
                new Planet { Name = "C" },
                new Planet { Name = "B" }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderBy($"{nameof(Planet.Name)} descending");

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderByDescending(p => p.Name);
            var inOrder = queryResult.Items.SequenceEqual(linqOrdered);

            Assert.True(inOrder);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndOrderDynamicallyByNameDescendingWithShorthand_ResultsAreCorrectlySorted()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A" },
                new Planet { Name = "C" },
                new Planet { Name = "B" }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderBy($"{nameof(Planet.Name)} desc");

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderByDescending(p => p.Name);
            var inOrder = queryResult.Items.SequenceEqual(linqOrdered);

            Assert.True(inOrder);
        }

        [IntegrationTest]
        public async Task WhenQuerying_AndOrderTwice_ResultsAreCorrectlySorted()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var planets = new[]
            {
                new Planet { Name = "A", SupportsLife = false },
                new Planet { Name = "C", SupportsLife = false },
                new Planet { Name = "B", SupportsLife = false },
                new Planet { Name = "A", SupportsLife = true },
                new Planet { Name = "C", SupportsLife = true },
                new Planet { Name = "B", SupportsLife = true }
            };
            foreach (var planet in planets)
            {
                await repository.CreateAsync(planet);
            }
            var planetIds = planets.Select(p => p.Id);
            var query = new Query<Planet>().Where(p => planetIds.Contains(p.Id)).OrderBy(p => p.Name).OrderBy(p => p.SupportsLife);

            var queryResult = await repository.GetAsync(query);

            var linqOrdered = queryResult.Items.OrderBy(p => p.Name).ThenBy(p => p.SupportsLife);
            var inOrder = queryResult.Items.SequenceEqual(linqOrdered);

            Assert.True(inOrder);
        }
    }

    [Collection(RepositoriesCollection.Name)]
    // ReSharper disable once InconsistentNaming Is the literal name of the vendor.
    public class SQLiteRepositoryTests : RepositoryTests<SQLiteRepositoryFixture>
    {
        public SQLiteRepositoryTests(SQLiteRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesMsSqlCollection.Name)]
    public class MsSqlRepositoryTests : RepositoryTests<MsSqlRepositoryFixture>
    {
        public MsSqlRepositoryTests(MsSqlRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }

    [Collection(RepositoriesOracleCollection.Name)]
    public class OracleRepositoryTests : RepositoryTests<OracleRepositoryFixture>
    {
        public OracleRepositoryTests(OracleRepositoryFixture repositoryFixture) : base(repositoryFixture) { }
    }
}
