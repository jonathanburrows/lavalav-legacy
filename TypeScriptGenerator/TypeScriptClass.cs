using System;
using System.Linq;
using lvl.TypescriptGenerator.Extensions;

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
            return IsAbstract ? "abstract " : string.Empty;
        }

        /// <summary>
        /// Will construct the necissary statement for inheritance.
        /// </summary>
        /// <returns>The typescript for extending a class.</returns>
        public string GetExtendStatement()
        {
            return BaseType == null ? string.Empty : $"extends {BaseType.Alias ?? BaseType.Name} ";
        }

        /// <summary>
        /// Will construct the statement for a classes properties.
        /// </summary>
        /// <returns>The generated statement for a class' properties.</returns>
        private string GetPropertyStatements()
        {
            var properties = Properties.Where(p => !p.IsInherited).Select(p =>
            {
                var decorators = p.Decorators.Select(d => d.ToTypeScript()).ToList();
                var decoratorsJoined = string.Join(", ", decorators);
                var decoratorStatement = !decorators.Any() ? "" : $"{decoratorsJoined} ";

                var abstractStatement = p.IsAbstract ? "abstract " : string.Empty;

                var typeIdentifier = p.PropertyType.Alias ?? p.PropertyType.Name;
                var collectionStatement = p.PropertyType.IsCollection ? "[]" : "";
                var typeStatement = $"{typeIdentifier}{collectionStatement}";

                var name = p.Name.ToPascal();

                return $"{decoratorStatement}public {abstractStatement}{name}: {typeStatement};";
            });
            return string.Join(Environment.NewLine + "    ", properties);
        }

        /// <summary>
        /// Will generate the constructor, with the options for properties.
        /// </summary>
        private string GetConstructor()
        {
            if (!Properties.Any() && BaseType == null)
            {
                return string.Empty;
            }
            else
            {
                var propertyAssignments = Properties.Where(p => !p.IsInherited).Select(property =>
                {
                    var name = property.Name.ToPascal();

                    if (property.PropertyType.IsPrimitive)
                    {
                        return $"this.{name} = options!.{name};";
                    }
                    else if (property.PropertyType.IsCollection)
                    {
                        return $"this.{name} = options!.{name}!.map(p => new {property.PropertyType.Name}(p));";
                    }
                    else
                    {
                        return $@"this.{name} = new {property.PropertyType.Name}(options!.{name});";
                    }
                });

                var baseConstructor = BaseType != null ? "super(options);\r\n        " : string.Empty;

                var propertyStatement = string.Join($"{Environment.NewLine}        ", propertyAssignments);

                return
$@"

    constructor(options?: I{Name}Options) {{
        options = options || {{}};
        {baseConstructor}{propertyStatement}
    }}";
            }
        }

        /// <summary>
        ///     Generates a strongly typed interface with all the properties of the class, so that it can be instantiated with fields.
        /// </summary>
        private string GetOptionsStatement()
        {
            var properties = Properties.Select(p => {
                var name = p.Name.ToPascal();
                var typeName = p.PropertyType.Alias ?? p.PropertyType.Name;
                var type = p.PropertyType.IsCollection ? $"{typeName}[]" : typeName;
                return $"{name}?: {type};";
            });

            var propertiesStatement = string.Join("\r\n    ", properties);

            return
$@"interface I{Name}Options {{
    {propertiesStatement}
}}";
        }

        /// <inheritdoc />
        public override string ToTypeScript()
        {
            return
$@"{GetImportStatements()}export {GetAbstractStatement()}class {Name}{GetGenericConstraintStatement()} {GetExtendStatement()}{GetImplementationStatements()}{{
    {GetPropertyStatements()}{GetConstructor()}
}}

{GetOptionsStatement()}
";
        }
    }
}
