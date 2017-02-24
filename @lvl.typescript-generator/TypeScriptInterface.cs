using System;
using System.Linq;
using lvl.TypescriptGenerator.Extensions;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Represents a typescript interface.
    /// </summary>
    internal class TypeScriptInterface : TypeScriptType
    {
        /// <summary>
        /// Will construct the statement for an interfaces properties.
        /// </summary>
        /// <returns>The generated statement for an interface's properties.</returns>
        private string GetPropertyStatements()
        {
            var properties = Properties.Select(p =>
            {
                var typeIdentifier = p.PropertyType.Alias ?? p.PropertyType.Name;
                var collectionStatement = p.PropertyType.IsCollection ? "[]" : "";
                var typeStatement = $"{typeIdentifier}{collectionStatement}";

                var optionalStatement = p.IsOptional ? "?" : "";

                var name = p.Name.ToPascal();

                return $"{name}{optionalStatement}: {typeStatement};";
            });
            return string.Join(Environment.NewLine + "    ", properties);
        }

        /// <inheritdoc />
        public override string ToTypeScript()
        {
            return
$@"{GetImportStatements()}export interface {Name} {GetImplementationStatements()}{{
    {GetPropertyStatements()}
}}";
        }
    }
}
