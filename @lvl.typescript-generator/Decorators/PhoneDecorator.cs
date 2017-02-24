using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Validates a phone address.
    /// </summary>
    internal class PhoneDecorator : TypeScriptType
    {
        public PhoneDecorator(string decoratorPath)
        {
            if (decoratorPath == null)
            {
                throw new ArgumentNullException(nameof(decoratorPath));
            }

            ModulePath = decoratorPath;
            Name = "Phone";
        }

        public override string ToTypeScript() => "@Phone()";
    }
}
