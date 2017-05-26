using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    ///     Provides URL validation. 
    /// </summary>
    internal class UrlDecorator : TypeScriptType
    {
        public UrlDecorator(string decoratorPath)
        {
            ModulePath = decoratorPath ?? throw new ArgumentNullException(nameof(decoratorPath));
            Name = "Url";
        }

        public override string ToTypeScript() => "@Url()";
    }
}
