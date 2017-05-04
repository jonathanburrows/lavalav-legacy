using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Validates an email address.
    /// </summary>
    internal class EmailAddressDecorator : TypeScriptType
    {
        public EmailAddressDecorator(string decoratorPath)
        {
            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Name = "EmailAddress";
        }

        public override string ToTypeScript() => "@EmailAddress()";
    }
}
