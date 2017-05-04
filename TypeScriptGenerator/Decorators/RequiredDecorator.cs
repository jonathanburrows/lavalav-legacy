using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Checks if a value has to be assigned to a property.
    /// </summary>
    internal class RequiredDecorator : TypeScriptType
    {
        public RequiredDecorator(string decoratorBin)
        {
            ModulePath = decoratorBin ?? throw new ArgumentNullException(nameof(decoratorBin));
            Name = "Required";
        }

        public override string ToTypeScript() => "@Required()";
    }
}
