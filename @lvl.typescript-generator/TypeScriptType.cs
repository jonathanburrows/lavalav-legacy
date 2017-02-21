using System;
using System.Collections.Generic;
using System.Text;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Represets a type in TypeScript.
    /// </summary>
    public abstract class TypeScriptType
    {
        /// <summary>The name of the type.</summary>
        public string Name { get; set; }

        /// <summary>Represents the next class in the inheritance chain</summary>
        public TypeScriptType BaseType { get; set; }

        /// <summary>Represents all the interfaces the type implements.</summary>
        public IEnumerable<TypeScriptType> Interfaces { get; set; } 

        /// <summary>Represents the path to the type's module.</summary>
        public string ModulePath { get; set; }

        /// <summary>Denotes if the type should be exported from the module.</summary>
        public bool IsVisible { get; set; }

        /// <summary>Represents the accessible properties for the type.</summary>
        public IEnumerable<TypeScriptProperty> Properties { get; set; }

        /// <summary>
        /// Will construct the necissary import statements.
        /// </summary>
        /// <returns>The typescript for importing the dependant libraries.</returns>
        protected StringBuilder GetImportStatements()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will resolve naming collisions
        /// </summary>
        private void MangleImports()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will return all types which need to be imported.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TypeScriptType> GetDependencies()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will construct the necissary statement for inheritance.
        /// </summary>
        /// <returns>The typescript for extending a class.</returns>
        public StringBuilder GetExtendStatement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will construct the necissary statements for interface implementation.
        /// </summary>
        /// <returns>The typescript for implementing interfaces.</returns>
        public StringBuilder GetImplementationStatements()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will return a string which contains the code for a typescript type.
        /// </summary>
        /// <returns>The contents of the typescript type.</returns>
        public abstract string ToTypeScript();
    }
}
