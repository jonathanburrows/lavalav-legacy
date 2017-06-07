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
        public async Task Get_collection_will_return_multiple_elements_when_theres_multiple_stored()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());

            var entities = await repository.GetAsync();

            Assert.True(entities.Any());
        }

        [IntegrationTest]
        public async Task Get_single_will_return_matching_element_when_it_exists()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var fetching = await repository.CreateAsync(new Moon());

            var fetched = await repository.GetAsync(fetching.Id);
            Assert.NotNull(fetched);
        }

        [IntegrationTest]
        public async Task Get_single_will_return_null_when_no_matching_entity_exists()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var fetched = await repository.GetAsync(0);

            Assert.Null(fetched);
        }

        [IntegrationTest]
        public async Task Generic_create_will_increase_size_by_one()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon());
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore + 1, countAfter);
        }

        [IntegrationTest]
        public async Task Create_will_increase_size_by_one()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var countBefore = (await repository.GetAsync()).Count();

            await repository.CreateAsync(new Moon());

            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countAfter, countBefore + 1);
        }

        [IntegrationTest]
        public async Task Generic_create_populates_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            var created = await repository.CreateAsync(new Moon());

            Assert.True(created.Id > 0);
        }

        [IntegrationTest]
        public async Task Create_populates_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var creating = new Moon();

            await repository.CreateAsync(creating);

            Assert.True(creating.Id > 0);
        }

        [IntegrationTest]
        public async Task Generic_create_will_throw_argument_null_exception_when_creating_is_null()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.CreateAsync(null));
        }

        [IntegrationTest]
        public async Task Craete_will_throw_argument_null_exception_when_creating_is_null()
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
        public async Task Create_will_throw_argument_exception_when_creating_is_not_repository_type()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentException>(() => repository.CreateAsync(new Planet()));
        }

        [IntegrationTest]
        public async Task Generic_create_will_throw_invalid_operation_exception_if_entity_already_has_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var entityWithIdentifier = new Moon { Id = 1 };

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.CreateAsync(entityWithIdentifier));
        }

        [IntegrationTest]
        public async Task Create_will_also_create_a_child_entity_if_the_child_doesnt_have_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var newChildEntity = new Planet();
            var creating = new Moon { Planet = newChildEntity };

            await repository.CreateAsync(creating);

            Assert.True(newChildEntity.Id > 0);
        }

        [IntegrationTest]
        public async Task Create_will_also_create_entities_in_child_collection_if_they_dont_have_ids()
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
        public async Task Create_will_update_child_entity_if_child_entity_has_id()
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
        public async Task Create_will_update_entities_in_child_collection_if_child_entity_already_has_id()
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
        public async Task Generic_update_will_update_properties_of_matching_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var original = await repository.CreateAsync(new Moon { Name = "Old Moon" });

            var updating = new Moon { Id = original.Id, Name = "New Moon" };
            await repository.UpdateAsync(updating);

            var updated = await repository.GetAsync(original.Id);
            Assert.Equal(updated.Name, updating.Name);
        }

        [IntegrationTest]
        public async Task Update_will_update_properties_of_matching_entity()
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
        public async Task Generic_update_will_throw_argument_null_exception_when_entity_is_null()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(null));
        }

        [IntegrationTest]
        public async Task Update_will_throw_argument_null_exception_when_entity_is_null()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(null));
        }

        [IntegrationTest]
        public async Task Update_will_throw_argument_exception_when_entity_isnt_of_repository_of_type()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(new Planet()));
        }

        [IntegrationTest]
        public async Task Generic_update_will_throw_invalid_operation_exception_when_theres_no_matching_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task Update_will_throw_invalid_operation_exception_when_theres_no_matching_entity()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.UpdateAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task Updating_will_create_child_entity_if_child_has_no_id()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var updating = await repository.CreateAsync(new Moon());
            updating.Planet = new Planet();

            await repository.UpdateAsync(updating);

            Assert.True(updating.Planet.Id > 0);
        }

        [IntegrationTest]
        public async Task Updating_will_create_entites_in_child_collection_if_they_dont_have_ids()
        {
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var updating = await planetRepository.CreateAsync(new Planet());

            var newChild = new Moon();
            updating.Moons = new[] { newChild };
            await planetRepository.UpdateAsync(updating);

            Assert.True(newChild.Id > 0);
        }

        [IntegrationTest]
        public async Task Updating_wull_update_child_entity_if_it_has_id()
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
        public async Task Updating_will_update_entities_in_child_collection_if_they_have_ids()
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
        public async Task Generic_delete_will_decrease_the_size_of_the_repository()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [IntegrationTest]
        public async Task Delete_will_decrease_the_size_of_the_repository()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());
            var countBefore = (await repository.GetAsync()).Count();

            await repository.DeleteAsync(deleting);
            var countAfter = (await repository.GetAsync()).Count();

            Assert.Equal(countBefore - 1, countAfter);
        }

        [IntegrationTest]
        public async Task Generic_delete_will_prevent_it_from_being_fetched_again()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            var deleting = new Moon();
            await repository.CreateAsync(deleting);

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [IntegrationTest]
        public async Task Delete_will_prevent_entity_from_being_fetched_again()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var deleting = await repository.CreateAsync(new Moon());

            await repository.DeleteAsync(deleting);
            var deleted = await repository.GetAsync(deleting.Id);

            Assert.Null(deleted);
        }

        [IntegrationTest]
        public async Task Generic_delete_will_throw_argument_null_exception_when_entity_is_null()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(null));
        }

        [IntegrationTest]
        public async Task Delete_will_throw_argument_null_exception_when_entity_is_null()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(null));
        }

        [IntegrationTest]
        public async Task Delete_will_throw_argument_exception_when_entity_isnt_of_repository_type()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var planet = new Planet();

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(planet));
        }

        [IntegrationTest]
        public async Task Generic_delete_will_throw_invalid_operation_exception_when_theres_no_matching_entity()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteAsync(new Moon()));
        }

        [IntegrationTest]
        public async Task Delete_will_throw_invalid_operation_exception_when_theres_no_matching_entity()
        {
            var repository = (IRepository)Services.GetRequiredService<IRepository<Moon>>();
            var unmatchedElement = new Moon { Id = int.MaxValue };

            await Assert.ThrowsAnyAsync<Exception>(() => repository.DeleteAsync(unmatchedElement));
        }

        // Wait, what?
        [IntegrationTest]
        public async Task WhenDeleting_WhenReferencingChild_ChildStillExists()
        {
            var moonRepository = Services.GetRequiredService<IRepository<Moon>>();
            var planetRepository = Services.GetRequiredService<IRepository<Planet>>();
            var deleting = new Moon { Planet = new Planet() };
            await moonRepository.CreateAsync(deleting);

            await moonRepository.DeleteAsync(deleting);

            var child = planetRepository.GetAsync(deleting.Planet.Id);
            Assert.NotNull(child);
        }

        [IntegrationTest]
        public async Task Delete_will_cascade_to_child_collections()
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
        public async Task Get_by_query_will_return_only_items_which_match_predicate()
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
        public async Task Get_by_query_will_not_return_any_items_that_dont_match_predicate()
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
        public async Task Get_by_query_will_only_return_matching_entities_when_using_dynamic_queries()
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
        public async Task Get_by_query_will_allow_to_lower_with_dynamic_filter()
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
        public async Task Get_query_allows_dynamic_filter_to_be_used_with_booleans()
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
        public async Task Get_by_query_will_return_all_entities_when_take_is_larger_than_total_matching_records()
        {
            var uniqueName = Guid.NewGuid().ToString();
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });
            var query = new Query<Planet>().Where(p => p.Name == uniqueName).Take(4);

            var queryResults = await repository.GetAsync(query);

            Assert.Equal(queryResults.Count, 3);
        }

        [IntegrationTest]
        public async Task Get_by_query_will_not_return_skipped_records()
        {
            var uniqueName = Guid.NewGuid().ToString();
            var skip = 1;
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            var skipped = await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });

            var query = new Query<Planet>().Where(p => p.Name == uniqueName).OrderBy(c => c.Id).Skip(skip);
            var queryResults = await repository.GetAsync(query);

            Assert.DoesNotContain(skipped, queryResults.Items);
        }

        [IntegrationTest]
        public async Task Get_by_query_will_return_a_collection_of_size_take_if_less_than_total_size_of_repository()
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
        public async Task Query_by_filter_will_return_correct_count_of_matched_items()
        {
            var uniqueName = Guid.NewGuid().ToString();
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = "not-matched" });

            var query = new Query<Planet>().Where(planet => planet.Name == uniqueName);
            var queryResults = await repository.GetAsync(query);

            Assert.Equal(2, queryResults.Count);
        }

        [IntegrationTest]
        public async Task Query_by_dynamic_filter_will_return_correct_count_of_matched_items()
        {
            var uniqueName = Guid.NewGuid().ToString();
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = uniqueName });
            await repository.CreateAsync(new Planet { Name = "not-matched" });

            var query = new Query<Planet>().Where($@"{nameof(Planet.Name)}=""{uniqueName}""");
            var queryResults = await repository.GetAsync(query);

            Assert.Equal(2, queryResults.Count);
        }

        [IntegrationTest]
        public async Task Query_by_will_return_count_of_entities_not_included_when_using_take()
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
        public async Task Query_by_will_return_count_of_entities_not_included_when_using_skip()
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
        public async Task Query_by_will_return_selected_properties()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = new Query<Planet>().Select(planet => planet.Name);

            var queryResult = await repository.GetAsync(query);

            Assert.Contains("Earth", queryResult.Items);
        }

        [IntegrationTest]
        public async Task Query_by_will_return_anonymous_object_with_selected_properties_if_supplied_with_anonymous_object()
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
        public async Task Query_by_will_return_child_property_when_selected()
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
        public async Task Query_by_will_correctly_order_by_name()
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
        public async Task Query_by_will_correctly_order_by_dynamic_name()
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
        public async Task Query_by_can_order_entities_by_descending()
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
        public async Task Query_by_can_order_by_descending_dynamically_with_the_word_descending()
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
        public async Task Query_by_can_order_by_descending_dynamically_with_the_word_desc()
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
        public async Task Query_by_can_order_by_twice()
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

    [Collection(nameof(RepositoriesCollection))]
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
