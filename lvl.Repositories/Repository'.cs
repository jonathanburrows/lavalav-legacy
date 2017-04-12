using lvl.Ontology;
using lvl.Repositories.Querying;
using Newtonsoft.Json;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Repositories
{
    /// <inheritdoc />
    public class Repository<TEntity> : IRepository<TEntity>, IRepository where TEntity : class, IEntity
    {
        private SessionProvider SessionProvider { get; }

        public Repository(SessionProvider sessionProvider)
        {
            SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TEntity>> GetAsync()
        {
            using (var session = SessionProvider.GetSession())
            {
                var entities = session.Query<TEntity>().ToList();
                return Task.FromResult(entities.AsEnumerable());
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAsync()
        {
            return await GetAsync();
        }

        /// <inheritdoc />
        public async Task<IQueryResult<TResult>> GetAsync<TResult>(IQuery<TEntity, TResult> query)
        {
            using (var session = SessionProvider.GetSession())
            {
                var unfiltered = session.Query<TEntity>();
                var items = query.Apply(unfiltered).ToList();
                var count = query.Count(unfiltered);
                var queryResult = new QueryResult<TResult>
                {
                    Count = count,
                    Items = items
                };

                // Force the collection to load, due to a limitation in asp.core serialization.
                // Please replace with something more elegant.
                var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                JsonConvert.SerializeObject(items, settings);

                return await Task.FromResult(queryResult);
            }
        }

        async Task<IQueryResult> IRepository.GetAsync(IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            dynamic boxedQuery = query;
            return await GetAsync(boxedQuery);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> GetAsync(int id)
        {
            using (var session = SessionProvider.GetSession())
            {
                var entity = session.Get<TEntity>(id);
                return Task.FromResult(entity);
            }
        }

        async Task<IEntity> IRepository.GetAsync(int id)
        {
            return await GetAsync(id);
        }

        /// <inheritdoc />
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
                // Since sqlite will use the same session, we need to make sure multiple transactions arent being used.
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

        async Task<IEntity> IRepository.CreateAsync(IEntity creating)
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

        /// <inheritdoc />
        public virtual Task<TEntity> UpdateAsync(TEntity updating)
        {
            if (updating == null)
            {
                throw new ArgumentNullException(nameof(updating));
            }

            using (var session = SessionProvider.GetSession())
            using (var transaction = session.BeginTransaction())
            {
                if (session.QueryOver<TEntity>().Where(x => x.Id == updating.Id).RowCount() == 0)
                {
                    throw new InvalidOperationException($"There exists no {typeof(TEntity).FullName} with the id of {updating.Id} to update");
                }
                session.Update(updating);
                transaction.Commit();
            }

            return Task.FromResult(updating);
        }

        async Task<IEntity> IRepository.UpdateAsync(IEntity updating)
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

        /// <inheritdoc />
        public virtual Task<TEntity> DeleteAsync(TEntity deleting)
        {
            if (deleting == null)
            {
                throw new ArgumentNullException(nameof(deleting));
            }

            using (var session = SessionProvider.GetSession())
            using (var transaction = session.BeginTransaction())
            {
                if (session.QueryOver<TEntity>().Where(x => x.Id == deleting.Id).RowCount() == 0)
                {
                    throw new InvalidOperationException($"There exists no {typeof(TEntity).FullName} with the id of {deleting.Id} to delete");
                }

                session.Delete(deleting);
                transaction.Commit();
            }

            return Task.FromResult(deleting);
        }

        async Task<IEntity> IRepository.DeleteAsync(IEntity deleting)
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
