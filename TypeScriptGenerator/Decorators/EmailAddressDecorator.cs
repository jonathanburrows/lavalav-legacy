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
            if (decoratorPath == null)
            {
                throw new ArgumentNullException(nameof(decoratorPath));
            }

            ModulePath = decoratorPath;
            Name = "EmailAddress";
        }

        public override string ToTypeScript() => "@EmailAddress()";
    }
}
