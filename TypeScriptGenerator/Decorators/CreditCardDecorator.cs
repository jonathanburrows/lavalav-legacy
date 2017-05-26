using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Specifies that a data field value is a credit card number.
    /// </summary>
    internal class CreditCardDecorator : TypeScriptType
    {
        public CreditCardDecorator(string decoratorPath)
        {
            Name = "CreditCard";
            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
        }

        public override string ToTypeScript() => "@CreditCard()";
    }
}
