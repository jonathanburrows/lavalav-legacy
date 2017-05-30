using System;

namespace lvl.Ontology.Authorization
{
    /// <summary>
    ///     Will prevent the api from performing operations on entities with this attribute.
    /// </summary>
    /// <remarks>
    ///     Intended for use in classes which are used exclusively in services, such as security classes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class HiddenFromApiAttribute: Attribute { }
}
