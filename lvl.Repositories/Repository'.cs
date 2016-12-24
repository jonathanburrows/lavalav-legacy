﻿using lvl.Ontology;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IRepository where TEntity : class, IEntity
    {
        private SessionManager SessionManager { get; }

        public Repository(SessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public virtual Task<IEnumerable<TEntity>> GetAsync()
        {
            using (var session = SessionManager.OpenSession())
            {
                var entities = session.Query<TEntity>().ToList();
                return Task.FromResult(entities.AsEnumerable());
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAsync()
        {
            return (await GetAsync()).Cast<IEntity>();
        }

        public virtual Task<TEntity> GetAsync(int id)
        {
            using (var session = SessionManager.OpenSession())
            {
                var entity = session.Get<TEntity>(id);
                return Task.FromResult(entity);
            }
        }

        async Task<IEntity> IRepository.GetAsync(int id)
        {
            return await GetAsync(id);
        }

        public virtual Task<TEntity> CreateAsync(TEntity creating)
        {
            if (creating == null) throw new ArgumentNullException(nameof(creating));

            using (var session = SessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(creating);
                transaction.Commit();
            }

            return Task.FromResult(creating);
        }

        async Task<IEntity> IRepository.CreateAsync(IEntity creating)
        {
            var boxed = creating as TEntity;
            if (creating == null) throw new ArgumentNullException(nameof(creating));
            if (boxed == null) throw new ArgumentException($"Cant create {creating.GetType().FullName} as a {typeof(TEntity).FullName}");

            return await CreateAsync(boxed);
        }

        public virtual Task<TEntity> UpdateAsync(TEntity updating)
        {
            if (updating == null) throw new ArgumentNullException(nameof(updating));

            using (var session = SessionManager.OpenSession())
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
            if (updating == null) throw new ArgumentNullException(nameof(updating));
            if (boxed == null) throw new ArgumentException($"Cant update {updating.GetType().FullName} as a {typeof(TEntity).FullName}");

            return await UpdateAsync(boxed);
        }

        public virtual Task<TEntity> DeleteAsync(TEntity deleting)
        {
            if (deleting == null) throw new ArgumentNullException(nameof(deleting));

            using (var session = SessionManager.OpenSession())
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
            if (deleting == null) throw new ArgumentNullException(nameof(deleting));
            if (boxed == null) throw new ArgumentException($"Cant delete {deleting.GetType().FullName} as a {typeof(TEntity).FullName}");

            return await DeleteAsync(boxed);
        }
    }
}