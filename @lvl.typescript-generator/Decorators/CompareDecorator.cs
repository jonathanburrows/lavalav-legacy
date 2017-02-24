using System;
using System.ComponentModel.DataAnnotations;
using lvl.TypescriptGenerator.Extensions;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Provides an attribute that compares two properties.
    /// </summary>
    internal class CompareDecorator : TypeScriptType
    {
        private string OtherProperty { get; }

        public CompareDecorator(string decoratorPath, CompareAttribute compareAttribute)
        {
            if (decoratorPath == null)
            {
                throw new ArgumentNullException(nameof(decoratorPath));
            }
            if (compareAttribute == null)
            {
                throw new ArgumentNullException(nameof(compareAttribute));
            }

            Name = "Compare";
            ModulePath = decoratorPath;
            OtherProperty = compareAttribute.OtherProperty.ToPascal();
        }

        public override string ToTypeScript() => $"@Compare('{OtherProperty}')";
    }
}
