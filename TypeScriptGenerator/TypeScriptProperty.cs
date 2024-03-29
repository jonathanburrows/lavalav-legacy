﻿using System.Collections.Generic;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    ///     Represents an accessible member on a class or interface.
    /// </summary>
    public class TypeScriptProperty
    {
        /// <summary>
        ///     The name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The type of the property.
        /// </summary>
        public TypeScriptType PropertyType { get; set; }

        /// <summary>
        ///     Validation metadata which will be added to classes.
        /// </summary>
        public IEnumerable<TypeScriptType> Decorators { get; set; }

        /// <summary>
        ///     Denotes on an interface if this is optional.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        ///     Denotes if this should be concrete or abstract.
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        ///     Denotes if the property is inherited from a base class.
        /// </summary>
        public bool IsInherited { get; set; }
    }
}
