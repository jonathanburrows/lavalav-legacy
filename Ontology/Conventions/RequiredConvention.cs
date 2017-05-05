using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    public class RequiredConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
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

        public void Apply(IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }
    }
}
