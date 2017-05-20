using IdentityServer4.Stores;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class ResourceStoreTests
    {
        private IRepository<ApiResourceEntity> ApiResourceRepository { get; }
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }
        private IResourceStore ResourceStore { get; }

        public ResourceStoreTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            ApiResourceRepository = serviceProvider.GetRequiredService<IRepository<ApiResourceEntity>>();
            IdentityResourceRepository = serviceProvider.GetRequiredService<IRepository<IdentityResourceEntity>>();
            ResourceStore = serviceProvider.GetRequiredService<IResourceStore>();
        }

        [Fact]
        public async Task Find_api_resource_will_throw_argument_null_exception_if_name_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ResourceStore.FindApiResourceAsync(null));
        }

        [Fact]
        public async Task Find_api_resource_will_throw_invalid_operation_exception_if_no_matching()
        {
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = "find-api-resource-mistmatch"
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => ResourceStore.FindApiResourceAsync("non-existant"));
        }

        [Fact]
        public async Task Find_api_resource_will_return_matching_api_resource()
        {
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = "find-api-resource"
            });

            var matchingApiResource = await ResourceStore.FindApiResourceAsync("find-api-resource");

            Assert.NotNull(matchingApiResource);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_throws_argument_null_exception_when_scopes_are_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ResourceStore.FindApiResourcesByScopeAsync(null));
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_empty_when_scopes_dont_match()
        {
            var name = "find-api-resource-by-scope-mismatched";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = name,
                Scopes = {
                    new ScopeEntity{ Name = name }
                }
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { "non-existant" });

            Assert.Empty(matchingApiResources);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_empty_when_no_scopes()
        {
            var name = "find-api-resource-by-scope-empty";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = name
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { "non-existant" });

            Assert.Empty(matchingApiResources);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_one_when_all_scopes_match()
        {
            var name = "find-api-resource-by-scope-multiple-matches";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = name,
                Scopes = {
                    new ScopeEntity{ Name = name },
                    new ScopeEntity{ Name = name }
                }
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { name });

            Assert.NotEmpty(matchingApiResources);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_one_when_second_scopes_match()
        {
            var name = "find-api-resource-by-scope-second-matches";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = name,
                Scopes = {
                    new ScopeEntity{ Name = name }
                }
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { "non-existant", name });

            Assert.NotEmpty(matchingApiResources);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_one_when_another_has_mismatch()
        {
            var name = "find-api-resource-by-scope-another-matches";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = $"{name}1",
                Scopes = {
                    new ScopeEntity{ Name = "another non matching" }
                }
            });
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = $"{name}2",
                Scopes = {
                    new ScopeEntity{ Name = name }
                }
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { name });

            Assert.NotEmpty(matchingApiResources);
        }

        [Fact]
        public async Task Find_api_resource_by_scope_will_return_one_when_some_scopes_match()
        {
            var name = "find-api-resource-by-scope-some-matches";
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = name,
                Scopes = {
                    new ScopeEntity{ Name = "another-non-matching" },
                    new ScopeEntity{ Name = name }
                }
            });

            var matchingApiResources = await ResourceStore.FindApiResourcesByScopeAsync(new[] { name });

            Assert.NotEmpty(matchingApiResources);
        }

        [Fact]
        public async Task Find_identity_resource_by_scope_throws_argument_null_exception_when_scopes_are_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ResourceStore.FindIdentityResourcesByScopeAsync(null));
        }

        [Fact]
        public async Task Find_identity_resource_by_scope_returns_empty_when_no_matching_names()
        {
            var name = "find-identity-resource-no-matching";
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = name
            });

            var matchingIdentityResources = await ResourceStore.FindIdentityResourcesByScopeAsync(new[] { "non-existant" });

            Assert.Empty(matchingIdentityResources);
        }

        [Fact]
        public async Task Find_identity_resource_by_scope_returns_one_when_name_matches()
        {
            var name = "find-identity-resource-no-matching";
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = name
            });
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = name + "-non-matching"
            });

            var matchingIdentityResources = await ResourceStore.FindIdentityResourcesByScopeAsync(new[] { name });

            Assert.Equal(matchingIdentityResources.Count(), 1);
        }

        [Fact]
        public async Task Find_identity_resource_by_scope_returns_multiple_when_many_names_match()
        {
            var name = "find-identity-resource-multiple-matching";
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = name
            });
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = name
            });

            var matchingIdentityResources = await ResourceStore.FindIdentityResourcesByScopeAsync(new[] { name });

            Assert.Equal(matchingIdentityResources.Count(), 2);
        }

        [Fact]
        public async Task Get_all_resources_returns_api_resources()
        {
            await ApiResourceRepository.CreateAsync(new ApiResourceEntity
            {
                Name = "get-all-resources-api"
            });

            var resources = await ResourceStore.GetAllResources();

            Assert.NotEmpty(resources.ApiResources);
        }

        [Fact]
        public async Task Get_all_resources_returns_identity_resources()
        {
            await IdentityResourceRepository.CreateAsync(new IdentityResourceEntity
            {
                Name = "get-all-resources-identity"
            });

            var resources = await ResourceStore.GetAllResources();

            Assert.NotEmpty(resources.IdentityResources);
        }
    }
}
