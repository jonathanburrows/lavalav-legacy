using FluentNHibernate.Conventions;
using System;
using FluentNHibernate.Conventions.Instances;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Provides a format of schema_tableName, so that tables can be grouped together.
    /// </summary>
    /// <remarks>
    ///     The normal schema was not modified due to high maintence of supporting multiple schemas with nhibernate.
    /// </remarks>
    public class TableNamingConvention : IClassConvention
    {
        /// <summary>
        ///     Formats the table name into schema_tableName, so that tables can be grouped together in the same schema.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <param name="instance">The table whos name will be formatted.</param>
        public void Apply(IClassInstance instance)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var entityType = instance.EntityType;

            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttribute?.Name ?? entityType.Name;

            var schemaAttribute = entityType.GetCustomAttribute<SchemaAttribute>();
            var schemaName = tableAttribute?.Schema ?? schemaAttribute?.Name ?? "lvl";

            instance.Table($"{schemaName}_{tableName}");
        }
    }
}
