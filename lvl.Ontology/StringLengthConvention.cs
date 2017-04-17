using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace lvl.Ontology
{

    public class StringLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(p => p.Type == typeof(string)).Expect(x => x.Length == 0);
        }

        public void Apply(IPropertyInstance instance)
        {
            const int defaultLength = 255;

            var property = instance.EntityType.GetProperty(instance.Name);
            var stringLengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
            var length = stringLengthAttribute?.MaximumLength ?? defaultLength;

            instance.Length(length);
        }
    }
}