using NHibernate.Cfg;
using NHibernate.Mapping;
using System;
using System.Linq;
using System.Reflection;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Configures entities that reference another entities by foreign key without an object reference.
    /// </summary>
    /// <remarks>
    ///     This changes configurations which should only be used by the database generators.
    /// </remarks>
    internal class ForeignKeyIdConvention
    {
        /// <summary>
        ///     Contains all the mapped classes which could contain foreign keys.
        /// </summary>
        private Configuration Configuration { get; }

        public ForeignKeyIdConvention(Configuration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        ///     Detects all properties decorated with [ForeignKeyId], and configures them to be foreign keys against their specified entities.
        /// </summary>
        public void AddForeignKeyIds()
        {
            foreach (var classMapping in Configuration.ClassMappings)
            {
                var properties = classMapping.MappedClass.GetProperties();
                foreach (var property in properties)
                {
                    var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyIdAttribute>();
                    if (foreignKeyAttribute != null)
                    {
                        if (Configuration.GetClassMapping(foreignKeyAttribute.ReferencedType) == null)
                        {
                            throw new InvalidOperationException($"Foreign key {classMapping.ClassName}.{property.Name} references the unmapped type {foreignKeyAttribute.ReferencedType.Name}.");
                        }

                        // Ambiguity can be caused if there is both an object and an id reference, so an exception is thrown.
                        if (properties.Any(p => p.PropertyType == foreignKeyAttribute.ReferencedType))
                        {
                            throw new InvalidOperationException($"{classMapping.ClassName} already has a foreign key object reference to {foreignKeyAttribute.ReferencedType.Name}.");
                        }

                        var foreignKeyProperty = classMapping.GetProperty(property.Name);
                        var foreignKeyColumns = foreignKeyProperty.ColumnIterator.Cast<Column>();

                        var parentTable = classMapping.RootTable;

                        var fullForeignKeyName = $"{parentTable.Name}_{property.Name}";

                        // the foreign key is hashed for consistency with nhibernates other foreign keys,
                        // and to make them short enough to work on multiple database vendors.
                        var hashedForeignKey = $"FK{fullForeignKeyName.GetHashCode().ToString().Replace("-", "")}";

                        parentTable.CreateForeignKey(hashedForeignKey, foreignKeyColumns, foreignKeyAttribute.ReferencedType.FullName);
                    }
                }
            }
        }
    }
}
