using System;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Used to denote a unique property.
    /// </summary>
    /// <remarks>
    ///     Intended to be used for database purposes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute { }
}
