using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Validates a phone address.
    /// </summary>
    internal class PhoneDecorator : TypeScriptType
    {
        public PhoneDecorator(string decoratorPath)
        {
            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Name = "Phone";
        }

        public override string ToTypeScript() => "@Phone()";
    }
}
