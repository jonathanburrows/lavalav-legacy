using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using lvl.Repositories;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories.Querying;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private IRepository<PersistedGrantEntity> PersistedGrantRepository { get; }

        public PersistedGrantStore(IRepository<PersistedGrantEntity> persistedGrantRepository)
        {
            PersistedGrantRepository = persistedGrantRepository ?? throw new ArgumentNullException(nameof(persistedGrantRepository));
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            var persistedGrants = await PersistedGrantRepository.GetAsync();
            return persistedGrants.Select(pg => pg.ToIdentityPersistedGrant());
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var query = new Query<PersistedGrantEntity>().Where(pg => pg.Key == key);
            var grants = await PersistedGrantRepository.GetAsync(query);
            return grants.Items.SingleOrDefault()?.ToIdentityPersistedGrant();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            var query = new Query<PersistedGrantEntity>()
                .Where(pg => pg.SubjectId == subjectId)
                .Where(pg => pg.ClientId == clientId);
            var grantsToRemove = await PersistedGrantRepository.GetAsync(query);

            foreach (var grantToRemove in grantsToRemove.Items)
            {
                await PersistedGrantRepository.DeleteAsync(grantToRemove);
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var query = new Query<PersistedGrantEntity>()
                .Where(pg => pg.SubjectId == subjectId)
                .Where(pg => pg.ClientId == clientId)
                .Where(pg => pg.Type == type);
            var grantsToRemove = await PersistedGrantRepository.GetAsync(query);

            foreach (var grantToRemove in grantsToRemove.Items)
            {
                await PersistedGrantRepository.DeleteAsync(grantToRemove);
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var query = new Query<PersistedGrantEntity>().Where(pg => pg.Key == key);
            var grantsToRemove = await PersistedGrantRepository.GetAsync(query);
            var grantToRemove = grantsToRemove.Items.Single();

            await PersistedGrantRepository.DeleteAsync(grantToRemove);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            if (grant == null)
            {
                throw new ArgumentNullException(nameof(grant));
            }

            var grantEntity = new PersistedGrantEntity
            {
                ClientId = grant.ClientId,
                CreationTime = grant.CreationTime,
                Data = grant.Data,
                Expiration = grant.Expiration,
                Key = grant.Key,
                SubjectId = grant.SubjectId,
                Type = grant.Type
            };

            await PersistedGrantRepository.CreateAsync(grantEntity);
        }
    }
}
