using System;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Provides a way to specify the schema without the table name.
    /// </summary>
    /// <remarks>
    ///     This was done as often the schema name will be specified without altering the table.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaAttribute : Attribute
    {
        /// <summary>
        ///     The of the schema which the entity will be placed in.
        /// </summary>
        public string Name { get; }

        public SchemaAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
