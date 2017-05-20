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
    /// <summary>
    ///     Interface for persisting any type of grant.
    /// </summary>
    internal class PersistedGrantStore : IPersistedGrantStore
    {
        private IRepository<PersistedGrantEntity> PersistedGrantRepository { get; }

        public PersistedGrantStore(IRepository<PersistedGrantEntity> persistedGrantRepository)
        {
            PersistedGrantRepository = persistedGrantRepository ?? throw new ArgumentNullException(nameof(persistedGrantRepository));
        }

        /// <summary>
        ///     Returns all grants with a matching subject id.
        /// </summary>
        /// <param name="subjectId">The unique string identifier of the subject, not to be confused with the database identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subjectId"/> is null.</exception>
        /// <returns>All persisted grants with a matching subject id.</returns>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            var subjectQuery = new Query<PersistedGrantEntity>().Where(g => g.SubjectId == subjectId);
            var persistedGrants = await PersistedGrantRepository.GetAsync(subjectQuery);

            return persistedGrants.Items.Select(pg => pg.ToIdentityPersistedGrant());
        }

        /// <summary>
        ///     Gets a grant with a matching key. If no matching grant is found, null is returned.
        /// </summary>
        /// <param name="key">The key which will be compared against the grant key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <returns>The grant with a matching key, if no match is found, then null.</returns>
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

        /// <summary>
        ///     Will remove all grants with a matching subject for a given client.
        /// </summary>
        /// <param name="subjectId">The string key of the subject, not to be confused with the database identifier.</param>
        /// <param name="clientId">The string identifier of the client, not to be confused with the database identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subjectId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientId"/> is null.</exception>
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


        /// <summary>
        ///     Will remove all grants with a matching subject and type for a given client.
        /// </summary>
        /// <param name="subjectId">The string key of the subject, not to be confused with the database identifier.</param>
        /// <param name="clientId">The string identifier of the client, not to be confused with the database identifier.</param>
        /// <param name="type">The string identifier of the client, not to be confused with the database identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subjectId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
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

        /// <summary>
        ///     Will remove all grants with the given key.
        /// </summary>
        /// <param name="key">The value which will be compared against the grant key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public async Task RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var query = new Query<PersistedGrantEntity>().Where(pg => pg.Key == key);
            var grantsToRemove = await PersistedGrantRepository.GetAsync(query);

            if (grantsToRemove.Items.Any())
            {
                var grantToRemove = grantsToRemove.Items.Single();
                await PersistedGrantRepository.DeleteAsync(grantToRemove);
            }
        }

        /// <summary>
        ///     Will save a grant.
        /// </summary>
        /// <param name="grant">The grant which will be persisted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="grant"/> is null.</exception>
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
