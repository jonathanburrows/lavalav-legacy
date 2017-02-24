using System;
using System.ComponentModel.DataAnnotations;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Specifies the numeric range constraints for the value of a data field.
    /// </summary>
    internal class RangeDecorator : TypeScriptType
    {
        private object Minimum { get; }
        private object Maximum { get; }

        public RangeDecorator(string decoratorPath, RangeAttribute rangeAttribute)
        {
            if (decoratorPath == null)
            {
                throw new ArgumentNullException(nameof(decoratorPath));
            }
            if (rangeAttribute == null)
            {
                throw new ArgumentNullException(nameof(rangeAttribute));
            }

            ModulePath = decoratorPath;
            Minimum = rangeAttribute.Minimum;
            Maximum = rangeAttribute.Maximum;
            Name = "Range";
        }

        public override string ToTypeScript() => $"@Range({Minimum}, {Maximum})";
    }
}
