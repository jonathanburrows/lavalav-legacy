using System;
using System.ComponentModel.DataAnnotations;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Specifies the minimum length of array or string data allowed in a property.
    /// </summary>
    internal class MinLengthDecorator : TypeScriptType
    {
        private int Length { get; }

        public MinLengthDecorator(string decoratorPath, MinLengthAttribute minLengthAttribute)
        {
            if (minLengthAttribute == null)
            {
                throw new ArgumentNullException(nameof(minLengthAttribute));
            }

            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Length = minLengthAttribute.Length;
            Name = "MinLength";
        }

        public override string ToTypeScript() => $"@MinLength({Length})";
    }
}
