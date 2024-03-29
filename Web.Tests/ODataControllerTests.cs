﻿using lvl.Ontology;
using lvl.Repositories;
using lvl.TestDomain;
using lvl.Web.OData;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using lvl.TestWebSite.Fixtures;
using Xunit;
using FluentNHibernate.Data;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class ODataControllerTests
    {
        private HttpClient Client { get; }
        private IServiceProvider Services { get; }

        public ODataControllerTests(WebHostFixture<TestWebSite.Startup> webHostFixture)
        {
            if (webHostFixture == null)
            {
                throw new ArgumentNullException(nameof(webHostFixture));
            }

            Client = webHostFixture.Client;
            Services = webHostFixture.Services;
        }

        [Fact]
        public async Task It_will_return_only_number_of_entities_that_are_requested_with_top_parameter()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            var top = 2;
            var url = $"/odata/{nameof(Moon)}?$top={top}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<Moon>>(responseDeserialized.Value.ToString());

            Assert.Equal(top, queryResult.Count);
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_invalid_top_is_given()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$top=hello";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_skip_then_number_of_entities_that_are_requested_with_skip_parameter()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            var skip = 2;
            var countBefore = (await repository.GetAsync()).Count();
            var url = $"/odata/{nameof(Moon)}?$skip={skip}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<Moon>>(responseDeserialized.Value.ToString());

            Assert.Equal(countBefore - skip, queryResult.Count);
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_invalid_skip_is_given()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$skip=hello";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_order_entities_when_an_orderby_param_is_given()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await EmptyRepositoryAsync(repository);
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
            var url = $"/odata/{nameof(Planet)}?$orderby={nameof(Planet.Name)}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<Planet>>(responseDeserialized.Value.ToString());

            var orderedResult = queryResult.OrderBy(p => p.Name);
            Assert.True(orderedResult.SequenceEqual(queryResult));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_orderby_is_invalid()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$orderby=hello";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_aggregate_exception_when_two_invalid_orderbys_are_given()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$orderby=hello,world";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_order_by_multiple_times_when_multiple_orderby_parameters_are_given()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await EmptyRepositoryAsync(repository);
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
            var url = $"/odata/{nameof(Planet)}?$orderby={nameof(Planet.Name)},{nameof(Planet.SupportsLife)}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<Planet>>(responseDeserialized.Value.ToString());

            // ReSharper disable once MultipleOrderBy
            var orderedResult = queryResult.OrderBy(p => p.Name).OrderBy(p => p.SupportsLife);
            Assert.True(orderedResult.SequenceEqual(queryResult));
        }

        [Fact]
        public async Task It_will_return_only_propertries_when_property_parameters_are_given()
        {
            var jsonOptions = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await EmptyRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Name = "Luna" });
            var url = $"/odata/{nameof(Moon)}?$select={nameof(Moon.Name)}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<SingleSelectResult>>(responseDeserialized.Value.ToString(), jsonOptions);

            var luna = queryResult.Single();
            Assert.Equal("Luna", luna.Name);
        }

        [Fact]
        public async Task It_will_return_multiple_properties_when_multiple_property_parameters_are_given()
        {
            var jsonOptions = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await EmptyRepositoryAsync(repository);
            var originalLuna = await repository.CreateAsync(new Moon { Name = "Luna" });
            var url = $"/odata/{nameof(Moon)}?$select={nameof(Moon.Id)},{nameof(Moon.Name)}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<DoubleSelectResult>>(responseDeserialized.Value.ToString(), jsonOptions);

            var luna = queryResult.Single();
            Assert.Equal(originalLuna.Name, luna.Name);
            Assert.Equal(originalLuna.Id, luna.Id);
        }

        [Fact]
        public async Task It_will_return_child_entity_when_property_parameter_is_child_entity()
        {
            var jsonOptions = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await EmptyRepositoryAsync(repository);
            var originalLuna = new Moon
            {
                Planet = new Planet { Name = "Terra" }
            };
            await repository.CreateAsync(originalLuna);
            var url = $"/odata/{nameof(Moon)}?$select={nameof(Moon.Planet)}";

            var response = await Client.GetAsync(url);
            var responseSerialized = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ODataResponse>(responseSerialized);
            var queryResult = JsonConvert.DeserializeObject<List<ChildSelectResult>>(responseDeserialized.Value.ToString(), jsonOptions);

            var luna = queryResult.Single();
            Assert.Equal(originalLuna.Planet.Name, luna.Planet.Name);
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_property_parameter_is_invalid()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$select=hello";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_aggregate_exception_when_multiple_property_parameters_are_invalid()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$select=hello,world";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_aggregate_exception_when_different_parameters_are_invalid()
        {
            var getUrl = $"/odata/{nameof(Moon)}?$skip=this&$orderby=invalid&$top=that&$select=hello";

            var getResult = await Client.GetAsync(getUrl);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        [Fact]
        public async Task It_will_throw_an_error_when_getting_entities_with_hide_from_api_attribute()
        {
            var url = $"/odata/{nameof(NasaApplication)}";

            var getResult = await Client.GetAsync(url);

            Assert.False(getResult.IsSuccessStatusCode);
        }

        private async Task EmptyRepositoryAsync<TEntity>(IRepository<TEntity> emptying) where TEntity : Entity, IAggregateRoot
        {
            foreach (var entity in await emptying.GetAsync())
            {
                await emptying.DeleteAsync(entity);
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local Used by reflection.
        // ReSharper disable UnusedAutoPropertyAccessor.Local Used by reflection
        private class SingleSelectResult
        {
            public string Name { get; set; }
        }

        private class DoubleSelectResult
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class ChildSelectResult
        {
            public Planet Planet { get; set; }
        }
    }
}
