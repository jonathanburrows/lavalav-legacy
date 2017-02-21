using System;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Will construct a typescript class.
    /// </summary>
    internal class TypeScriptClass : TypeScriptType
    {
        /// <summary> Denotes if the class can be instantiated or not. </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Will return the statement for making a class abstract.
        /// </summary>
        /// <returns>The generated typescript for making a class abstract.</returns>
        private string GetAbstractStatement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will construct the statement for a classes properties.
        /// </summary>
        /// <returns>The generated statement for a class' properties.</returns>
        private string GetPropertyStatements()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ToTypeScript()
        {
            throw new NotImplementedException();
        }
    }
}
