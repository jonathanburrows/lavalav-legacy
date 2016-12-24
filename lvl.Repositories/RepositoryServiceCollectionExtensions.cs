﻿using lvl.Repositories;
using NHibernate;
using NHibernate.Dialect;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a method to register all classes used to resolve repositories from a service provider
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all classes used to resolve repositories.
        /// </summary>
        /// <param name="serviceCollection">The service collection which will have the types registered against it</param>
        /// <returns>The given service collection, with the registered types.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> cannot be null.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="serviceCollection"/> hasnt had AddDomains called.</exception>
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException();

            var services = serviceCollection.BuildServiceProvider();
            var configuration = services.GetService<NHibernate.Cfg.Configuration>();
            if (configuration == null)
            {
                throw new InvalidOperationException($"{nameof(ServiceCollection)} has not had {nameof(DomainServiceCollectionExtensions.AddDomains)} called before {nameof(RepositoryServiceCollectionExtensions.AddRepositories)}");
            }

            serviceCollection.AddScoped<TypeResolver>();
            serviceCollection.AddScoped<RepositoryFactory>();
            serviceCollection.AddScoped<ISessionFactory>(_ => configuration.BuildSessionFactory());

            var mappedTypes = configuration.ClassMappings.Select(c => c.MappedClass);
            var genericIRepositoryType = typeof(IRepository<>);
            var genericRepositoryType = typeof(Repository<>);
            foreach (var mappedType in mappedTypes)
            {
                var castedIRepositoryType = genericIRepositoryType.MakeGenericType(mappedType);
                var castedRepositoryType = genericRepositoryType.MakeGenericType(mappedType);
                serviceCollection.AddScoped(castedIRepositoryType, castedRepositoryType);
            }

            var dialectKey = NHibernate.Cfg.Environment.Dialect;
            var dialect = configuration.GetProperty(dialectKey);
            var isSqlLite = dialect.Equals(typeof(SQLiteDialect).AssemblyQualifiedName);
            var sessionManager = isSqlLite ? typeof(SQLitePersistentSessionManager) : typeof(SessionManager);
            serviceCollection.AddScoped(typeof(SessionManager), sessionManager);

            return serviceCollection;
        }
    }
}