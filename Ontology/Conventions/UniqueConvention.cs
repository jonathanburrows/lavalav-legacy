using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Will mark database fields as unique, to help database integrity.
    /// </summary>
    internal class UniqueConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        /// <summary>
        ///     Determine if a property should be marked as unique.
        ///     
        ///     If it has a unique attribute, yes; otherwise, no.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="criteria"/> is null.</exception>
        /// <param name="criteria">Instace that could be supplied.</param>
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            criteria.Expect(propertyInspector =>
            {
                var parentType = propertyInspector.Property.DeclaringType;
                var propertyInfo = parentType.GetProperty(propertyInspector.Property.Name);
                return propertyInfo.GetCustomAttribute<UniqueAttribute>() != null;
            });
        }

        /// <summary>
        ///     Marks the property as unique.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <param name="instance">The property which will be marked.</param>
        public void Apply(IPropertyInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instance.Unique();
        }
    }
}
