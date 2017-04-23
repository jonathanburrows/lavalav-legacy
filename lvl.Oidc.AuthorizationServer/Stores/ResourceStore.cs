using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System.Linq;
using lvl.Oidc.AuthorizationServer.Models;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    public class ResourceStore : IResourceStore
    {
        private IRepository<ApiResourceEntity> ApiResourceRepository { get; }
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }

        public ResourceStore(IRepository<ApiResourceEntity> apiResourceRepository, IRepository<IdentityResourceEntity> identityResourceRepository)
        {
            ApiResourceRepository = apiResourceRepository ?? throw new ArgumentNullException(nameof(apiResourceRepository));
            IdentityResourceRepository = identityResourceRepository ?? throw new ArgumentNullException(nameof(identityResourceRepository));
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var query = new Query<ApiResourceEntity>().Where(ar => ar.Name == name);
            var apiResources = await ApiResourceRepository.GetAsync(query);
            return apiResources.Items.Single().ToIdentityServer();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            var query = new Query<ApiResourceEntity>().Where(api => api.Scopes.Where(x => scopeNames.Contains(x.Name)).Any());
            var apiResources = await ApiResourceRepository.GetAsync(query);
            return apiResources.Items.Select(ar => ar.ToIdentityServer());
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            var query = new Query<IdentityResourceEntity>().Where(ir => scopeNames.Contains(ir.Name));
            var identityResources = await IdentityResourceRepository.GetAsync(query);
            return identityResources.Items.Select(ir => ir.ToIdentityServer());
        }

        public async Task<Resources> GetAllResources()
        {
            var identityResources = await IdentityResourceRepository.GetAsync();
            var boxedIdentityResources = identityResources.Select(ir => ir.ToIdentityServer()).ToList();

            var apiResources = await ApiResourceRepository.GetAsync();
            var boxedApiResources = apiResources.Select(ar => ar.ToIdentityServer()).ToList();

            return new Resources(boxedIdentityResources, boxedApiResources);
        }
    }
}
