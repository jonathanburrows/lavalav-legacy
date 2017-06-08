using FluentNHibernate.Data;
using lvl.Ontology;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using System;
using System.Linq;
using System.Reflection;

namespace lvl.Repositories
{
    /// <summary>
    ///     Provides ways to construct repositories.
    /// </summary>
    public class RepositoryFactory
    {
        private Configuration Configuration { get; }

        /// <summary>
        ///     This was done so that generic repositories can be resolved and overridden
        ///     with the same logic as the repository factory.
        /// </summary>
        private IServiceProvider Services { get; }

        /// <summary>
        ///     Used to make sure reflection calls dont occur each construction
        /// </summary>
        private static MethodInfo GenericConstructMethod { get; } = typeof(RepositoryFactory).GetMethod(nameof(RepositoryFactory.Construct), new Type[0]);

        public RepositoryFactory(Configuration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        ///     Creates a repository of the given type.
        /// </summary>
        /// <typeparam name="TEntity">The type of repository to be constructed.</typeparam>
        /// <returns>The constructed repository.</returns>
        /// <exception cref="InvalidOperationException">The entity type is not mapped to nhibernate.</exception>
        public IRepository<TEntity> Construct<TEntity>() where TEntity : Entity, IAggregateRoot
        {
            var mappedClasses = Configuration.ClassMappings.Select(c => c.MappedClass);
            if (!mappedClasses.Contains(typeof(TEntity)))
            {
                throw new InvalidOperationException($"{typeof(TEntity).FullName} has not been mapped.");
            }

            return Services.GetRequiredService<IRepository<TEntity>>();
        }

        /// <summary>
        ///     Creates a repository of the given type.
        /// </summary>
        /// <param name="type">The type of the repostory to be constructed.</param>
        /// <returns>The constructed repository.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="type"/> does not implement IEntity</exception>
        /// <exception cref="InvalidOperationException"><paramref name="type"/> has not been mapped to nhibernate.</exception> 
        public IRepository Construct(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            else if (!typeof(Entity).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type.FullName} does not inherit from {nameof(Entity)}");
            }

            var castedConstructMethod = GenericConstructMethod.MakeGenericMethod(type);
            var constructed = (IRepository)castedConstructMethod.Invoke(this, new object[0]);
            return constructed;
        }
    }
}
