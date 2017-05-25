using lvl.Ontology;
using lvl.Repositories.Authorization;
using lvl.Repositories.Querying;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Repositories
{
    /// <summary>
    ///     Manages persistence f0r a set of entities of a certain type.
    /// </summary>
    /// <typeparam name="TEntity">The type of all entities in the repository.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity>, IRepository where TEntity : Entity, IAggregateRoot
    {
        private SessionProvider SessionProvider { get; }
        private AggregateRootFilter AggregateRootFilter { get; }

        public Repository(SessionProvider sessionProvider, AggregateRootFilter aggregateRootFilter)
        {
            SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            AggregateRootFilter = aggregateRootFilter ?? throw new ArgumentNullException(nameof(aggregateRootFilter));
        }

        /// <summary>
        ///     Gets all entities in the repository.
        /// </summary>
        /// <returns>All entities in the repository.</returns>
        public virtual Task<IEnumerable<TEntity>> GetAsync()
        {
            using (var session = SessionProvider.GetSession())
            {
                var entities = session.Query<TEntity>();
                var authorized = AggregateRootFilter.Filter(entities).ToList();
                return Task.FromResult(authorized.AsEnumerable());
            }
        }

        /// <summary>
        ///     Gets all entities in the repository.
        /// </summary>
        /// <returns>All entities in the repository.</returns>
        async Task<IEnumerable<Entity>> IRepository.GetAsync()
        {
            return await GetAsync();
        }

        /// <summary>
        ///     Applies an odata query to the entities, and returns the result.
        /// </summary>
        /// <param name="query">The query to be applied to the entities.</param>
        /// <returns>The result of the odata query.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is null.</exception>
        public async Task<IQueryResult<TResult>> GetAsync<TResult>(IQuery<TEntity, TResult> query)
        {
            if(query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            using (var session = SessionProvider.GetSession())
            {
                var unfiltered = session.Query<TEntity>();
                var authorized = AggregateRootFilter.Filter(unfiltered);
                var items = query.Apply(authorized).ToList();
                var count = query.Count(authorized);
                var queryResult = new QueryResult<TResult>
                {
                    Count = count,
                    Items = items
                };

                return await Task.FromResult(queryResult);
            }
        }

        /// <summary>
        ///     Applies an odata query to the entities, and returns the result.
        /// </summary>
        /// <param name="query">The query to be applied to the entities.</param>
        /// <returns>The result of the odata query.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is null.</exception>
        async Task<IQueryResult> IRepository.GetAsync(IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            dynamic boxedQuery = query;
            return await GetAsync(boxedQuery);
        }

        /// <summary>
        ///     Gets an entity with a matching id from the repository.
        /// </summary>
        /// <param name="id">The identifier of the desired entity.</param>
        /// <returns>The matching entity if one exists, null if no matching entity.</returns>
        public virtual Task<TEntity> GetAsync(int id)
        {
            using (var session = SessionProvider.GetSession())
            {
                var allEntities = session.Query<TEntity>();
                var authorized = AggregateRootFilter.Filter(allEntities);
                var matchingEntity = authorized.SingleOrDefault(entity => entity.Id == id);
                return Task.FromResult(matchingEntity);
            }
        }

        /// <summary>
        ///     Gets an entity with a matching id from the repository.
        /// </summary>
        /// <param name="id">The identifier of the desired entity.</param>
        /// <returns>The matching entity if one exists, null if no matching entity.</returns>
        async Task<Entity> IRepository.GetAsync(int id)
        {
            return await GetAsync(id);
        }

        /// <summary>
        ///     Creates an entity with all properties, and updates the model with any generated values.
        /// </summary>
        /// <param name="creating">The entity to be added.</param>
        /// <returns>The model with all generated values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="creating"/> cannot be null.</exception>
        public virtual Task<TEntity> CreateAsync(TEntity creating)
        {
            if (creating == null)
            {
                throw new ArgumentNullException(nameof(creating));
            }
            if (creating.Id > 0)
            {
                throw new InvalidOperationException($"Cannot create {typeof(TEntity).Name} as it already has an id: {creating.Id}");
            }

            using (var session = SessionProvider.GetSession())
            {
                // Locked because SQLite will use the same connection, with the same transaction, and then complete the transaction,
                // even when there are multiple threads using it but not yet complete.
                lock (session.Connection)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.Save(creating);
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(creating);
        }

        /// <summary>
        ///     Creates an entity with all properties, and updates the model with any generated values.
        /// </summary>
        /// <param name="creating">The entity to be added.</param>
        /// <returns>The model with all generated values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="creating"/> cannot be null.</exception>
        /// <exception cref="ArgumentException"><paramref name="creating"/> is not the type of the repository.</exception>
        async Task<Entity> IRepository.CreateAsync(Entity creating)
        {
            var boxed = creating as TEntity;
            if (creating == null)
            {
                throw new ArgumentNullException(nameof(creating));
            }
            if (boxed == null)
            {
                throw new ArgumentException($"Cant create {creating.GetType().FullName} as a {typeof(TEntity).FullName}");
            }

            return await CreateAsync(boxed);
        }

        /// <summary>
        ///     Updates an entity, whos identifier matches the given model, with all the model's fields.
        /// </summary>
        /// <param name="updating">The model whos properties will be applied to the matching entity.</param>
        /// <returns>The model with updated properties which were generated.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="updating"/> cannot be null.</exception>
        /// <exception cref="InvalidOperationException">There exists no entity with a matching id.</exception>
        public virtual Task<TEntity> UpdateAsync(TEntity updating)
        {
            if (updating == null)
            {
                throw new ArgumentNullException(nameof(updating));
            }

            using (var session = SessionProvider.GetSession())
            {
                // Locked because SQLite will use the same connection, with the same transaction, and then complete the transaction,
                // even when there are multiple threads using it but not yet complete.
                lock (session.Connection)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        if (session.QueryOver<TEntity>().Where(x => x.Id == updating.Id).RowCount() == 0)
                        {
                            throw new InvalidOperationException($"There exists no {typeof(TEntity).FullName} with the id of {updating.Id} to update");
                        }
                        session.Update(updating);
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(updating);
        }

        /// <summary>
        ///     Updates an entity, whos identifier matches the given model, with all the model's fields.
        /// </summary>
        /// <param name="updating">The model whos properties will be applied to the matching entity.</param>
        /// <returns>The model with updated properties which were generated.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="updating"/> cannot be null.</exception>
        /// <exception cref="InvalidOperationException">There exists no entity with a matching id.</exception>
        /// <exception cref="ArgumentException"><paramref name="updating"/> is not the type of the repository.</exception>
        async Task<Entity> IRepository.UpdateAsync(Entity updating)
        {
            var boxed = updating as TEntity;
            if (updating == null)
            {
                throw new ArgumentNullException(nameof(updating));
            }
            if (boxed == null)
            {
                throw new ArgumentException($"Cant update {updating.GetType().FullName} as a {typeof(TEntity).FullName}");
            }

            return await UpdateAsync(boxed);
        }

        /// <summary>
        ///     Deletes an entity, whos identifier matches the given model.
        /// </summary>
        /// <param name="deleting">The model which was deleted.</param>
        /// <returns>The model which was deleted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="deleting"/> cannot be null.</exception>
        /// <exception cref="InvalidOperationException">There exists no entity with a matching id.</exception>
        public virtual Task<TEntity> DeleteAsync(TEntity deleting)
        {
            if (deleting == null)
            {
                throw new ArgumentNullException(nameof(deleting));
            }

            using (var session = SessionProvider.GetSession())
            {
                // Locked because SQLite will use the same connection, with the same transaction, and then complete the transaction,
                // even when there are multiple threads using it but not yet complete.
                lock (session.Connection)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        if (session.QueryOver<TEntity>().Where(x => x.Id == deleting.Id).RowCount() == 0)
                        {
                            throw new InvalidOperationException($"There exists no {typeof(TEntity).FullName} with the id of {deleting.Id} to delete");
                        }

                        session.Delete(deleting);
                        transaction.Commit();
                    }
                }
            }

            return Task.FromResult(deleting);
        }

        /// <summary>
        ///     Deletes an entity, whos identifier matches the given model.
        /// </summary>
        /// <param name="deleting">The model which was deleted.</param>
        /// <returns>The model which was deleted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="deleting"/> cannot be null.</exception>
        /// <exception cref="InvalidOperationException">There exists no entity with a matching id.</exception>
        /// <exception cref="ArgumentException"><paramref name="deleting"/> is not the type of the repository.</exception>
        async Task<Entity> IRepository.DeleteAsync(Entity deleting)
        {
            var boxed = deleting as TEntity;
            if (deleting == null)
            {
                throw new ArgumentNullException(nameof(deleting));
            }
            if (boxed == null)
            {
                throw new ArgumentException($"Cant delete {deleting.GetType().FullName} as a {typeof(TEntity).FullName}");
            }

            return await DeleteAsync(boxed);
        }
    }
}
