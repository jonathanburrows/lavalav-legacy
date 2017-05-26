using System;
using System.ComponentModel.DataAnnotations;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Specifies that a data field value in ASP.NET Dynamic Data must match the specified regular expression.
    /// </summary>
    public class RegularExpressionDecorator : TypeScriptType
    {
        private string Pattern { get; }

        public RegularExpressionDecorator(string decoratorPath, RegularExpressionAttribute regularExpressionAttribute)
        {
            if (regularExpressionAttribute == null)
            {
                throw new ArgumentNullException(nameof(regularExpressionAttribute));
            }

            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Pattern = regularExpressionAttribute.Pattern;
            Name = "RegularExpression";
        }

        public override string ToTypeScript() => $"@RegularExpression(/{Pattern}/)";
    }
}
