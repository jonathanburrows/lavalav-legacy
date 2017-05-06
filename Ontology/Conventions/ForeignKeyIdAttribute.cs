using System;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Allows an entity to reference another entity by foreign key without an object reference.
    /// </summary>
    /// <remarks>
    ///     This is used primarily by the database generators to create foreign keys, and maintain integrety.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyIdAttribute : Attribute
    {
        /// <summary>
        ///     The entity which is being referenced by the foreign key.
        /// </summary>
        internal Type ReferencedType { get; }

        public ForeignKeyIdAttribute(Type referencedType)
        {
            ReferencedType = referencedType ?? throw new ArgumentNullException(nameof(referencedType));
        }
    }
}
