using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Will mark database fields as not null, to prevent the database generator from damaging integrity.
    /// </summary>
    public class RequiredConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        /// <summary>
        ///     Determines if a property should be marked as not-null.
        ///     
        ///     If the property is a string or a nullable value, then it will be marked as required
        ///     if it is decorated with the RequiredAttribute.
        ///     
        ///     If the property is an object, then it will not be required.
        ///     
        ///     If the property is a non nullable value, then it will always be required.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="criteria"/> is null.</exception>
        /// <param name="criteria">Instace that could be supplied.</param>
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            if(criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            criteria.Expect(propertyInspector =>
            {
                var propertyType = propertyInspector.Property.PropertyType;
                var parentType = propertyInspector.Property.DeclaringType;

                if (propertyType == typeof(string))
                {
                    var propertyInfo = parentType.GetProperty(propertyInspector.Name);
                    return propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
                }
                else if (!propertyType.IsValueType)
                {
                    return false;
                }
                else if (Nullable.GetUnderlyingType(propertyType) == null)
                {
                    return true;
                }
                else
                {
                    var propertyInfo = parentType.GetProperty(propertyInspector.Name);
                    return propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
                }
            });
        }

        /// <summary>
        ///     Marks the property as not null.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <param name="instance">The property which will be marked.</param>
        public void Apply(IPropertyInstance instance)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instance.Not.Nullable();
        }
    }
}
