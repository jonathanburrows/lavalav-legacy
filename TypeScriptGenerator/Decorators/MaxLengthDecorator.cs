using System;
using System.ComponentModel.DataAnnotations;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Specifies the maximum length of array or string data allowed in a property.
    /// </summary>
    internal class MaxLengthDecorator : TypeScriptType
    {
        private int Length { get; }

        public MaxLengthDecorator(string decoratorPath, MaxLengthAttribute maxLengthAttribute)
        {
            if (maxLengthAttribute == null)
            {
                throw new ArgumentNullException(nameof(maxLengthAttribute));
            }

            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Length = maxLengthAttribute.Length;
            Name = "MaxLength";
        }

        public override string ToTypeScript() => $"@MaxLength({Length})";
    }
}
