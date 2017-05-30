using lvl.Ontology;
using lvl.Ontology.Authorization;
using NHibernate.Cfg;
using System;
using System.Linq;
using System.Reflection;

namespace lvl.Repositories
{
    /// <summary>
    ///     Converts strings to mapped types.
    /// </summary>
    public class TypeResolver
    {
        private Configuration Configuration { get; }

        public TypeResolver(Configuration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        ///     Gets the mapped class with a matching name.
        /// </summary>
        /// <param name="entityType">
        ///     The name of the type to get. 
        ///     May be either a fully qualified name, or just the class name</param>
        /// <returns>
        ///     The mapped type with the given name
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityType"/> cannot be null</exception>
        /// <exception cref="InvalidOperationException">No class with that name was mapped</exception>
        /// <exception cref="InvalidOperationException">More than one class with that name was mapped</exception>
        /// <exception cref="InvalidOperationException">Entity type does not implement IAggregateRoot</exception>
        /// <exception cref="InvalidOperationException">Entity type is decorated with HiddenFromApiAttribute.</exception>
        /// <remarks>
        ///     Name comparisons are case insensitive.
        ///     Fully qualified names take presidence over class names
        /// </remarks>
        public virtual Type Resolve(string entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var types = Configuration.ClassMappings.Select(c => c.MappedClass).ToArray();

            var matchingFullyQualified = types.Where(t => t.FullName.Equals(entityType, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (matchingFullyQualified.Count > 1)
            {
                throw new InvalidOperationException($"More than one entity with the full qualified name {entityType} was mapped");
            }
            else if (matchingFullyQualified.Any())
            {
                return matchingFullyQualified.Single();
            }

            var matchingClasses = types.Where(t => t.Name.Equals(entityType, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (matchingClasses.Count > 1)
            {
                throw new InvalidOperationException($"More than one entity with the name {entityType} was mapped");
            }
            else if (!matchingClasses.Any())
            {
                throw new InvalidOperationException($"No entity was mapped with the class or fully qualified name {entityType}");
            }
            else if (!typeof(IAggregateRoot).IsAssignableFrom(matchingClasses.Single()))
            {
                throw new InvalidOperationException($"{entityType} does not implement {nameof(IAggregateRoot)}");
            }
            else if (matchingClasses.Single().GetCustomAttribute<HiddenFromApiAttribute>() != null)
            {
                throw new InvalidOperationException($"{entityType} cannot be accessed through the api.");
            }
            else
            {
                return matchingClasses.Single();
            }
        }
    }
}
