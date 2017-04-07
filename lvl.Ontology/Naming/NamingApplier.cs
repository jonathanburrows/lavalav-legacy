using NHibernate.Mapping;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace lvl.Ontology.Naming
{
    internal class NamingApplier
    {
        public void Apply(PersistentClass persistentClass)
        {
            var table = persistentClass.Table;
            var type = persistentClass.MappedClass;

            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttribute?.Name ?? table.Name;

            var schemaAttribute = type.GetCustomAttribute<SchemaAttribute>();
            var schemaName = tableAttribute?.Schema ?? schemaAttribute?.Name ?? table.Schema ?? "lvl";
            table.Name = $"{schemaName}_{tableName}";
        }
    }
}
