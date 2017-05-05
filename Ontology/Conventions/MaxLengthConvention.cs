using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Will set the database precision for strings, to prevent overflows from happening.
    /// </summary>
    public class MaxLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        /// <summary>
        ///     Will be used to set all string lengths which arent explicitly set.
        /// </summary>
        public const int DefaultLength = 1024;

        /// <summary>
        ///     Determines if a property needs a string length specified for the database.
        ///     
        ///     If the property is a string, then yes. Otherwise, no.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="criteria"/> is null.</exception>
        /// <param name="criteria">Instace that could be supplied.</param>
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            criteria.Expect(propertyInspector => propertyInspector.Property.PropertyType == typeof(string));
        }

        /// <summary>
        ///     Gives the string value a length.
        ///     
        ///     If the property has a MaxLengthAttribute, then it's value is used. Otherwise, the default length is set.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="criteria"/> is null.</exception>
        /// <param name="instance">The property which will be marked.</param>
        public void Apply(IPropertyInstance instance)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var parentType = instance.Property.DeclaringType;
            var propertyInfo = parentType.GetProperty(instance.Property.Name);
            var maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
            var maxLength = maxLengthAttribute?.Length ?? DefaultLength;

            instance.Length(maxLength);
        }
    }
}
