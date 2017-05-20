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
    /// <summary>
    ///     Retrieves resources from nhibernate stores.
    /// </summary>
    internal class ResourceStore : IResourceStore
    {
        private IRepository<ApiResourceEntity> ApiResourceRepository { get; }
        private IRepository<IdentityResourceEntity> IdentityResourceRepository { get; }

        public ResourceStore(IRepository<ApiResourceEntity> apiResourceRepository, IRepository<IdentityResourceEntity> identityResourceRepository)
        {
            ApiResourceRepository = apiResourceRepository ?? throw new ArgumentNullException(nameof(apiResourceRepository));
            IdentityResourceRepository = identityResourceRepository ?? throw new ArgumentNullException(nameof(identityResourceRepository));
        }

        /// <summary>
        ///     Fetches an api resource with a given name.
        /// </summary>
        /// <param name="name">The name of the api resource which is being searched for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No api resource with that name was found.</exception>
        /// <exception cref="InvalidOperationException">Multiple api resources with the given name were found.</exception>
        /// <returns>The matching api resource.</returns>
        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var query = new Query<ApiResourceEntity>().Where(ar => ar.Name == name);
            var apiResources = await ApiResourceRepository.GetAsync(query);

            if (apiResources.Count == 0)
            {
                throw new InvalidOperationException($"No api resources found with the name '${name}'.");
            }
            if(apiResources.Count > 1)
            {
                throw new InvalidOperationException($"{apiResources.Count} api resources were found with the name '${name}'.");
            }

            return apiResources.Items.Single().ToIdentityServer();
        }

        /// <summary>
        ///     Finds all api resources that have at least one of the given scopes.
        /// </summary>
        /// <param name="scopeNames">The scopes which each api must have at least one of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="scopeNames"/> is null.</exception>
        /// <returns>All api resources which have at least one of the given scopes.</returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            var query = new Query<ApiResourceEntity>().Where(api => api.Scopes.Any(x => scopeNames.Contains(x.Name)));
            var apiResources = await ApiResourceRepository.GetAsync(query);
            return apiResources.Items.Select(ar => ar.ToIdentityServer());
        }

        /// <summary>
        ///     Finds all identity resources whos name is in the list of given scopes.
        /// </summary>
        /// <param name="scopeNames">The scopes which each identity must have a matching name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="scopeNames"/> is null.</exception>
        /// <returns>All identity resources whos name is in the given list of scopes.</returns>
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

        /// <summary>
        ///     Gets the combination of all identity and api resources.
        /// </summary>
        /// <returns>All resources combined.</returns>
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
