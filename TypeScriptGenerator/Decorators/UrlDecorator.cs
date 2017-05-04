using System;

namespace lvl.TypescriptGenerator.Decorators
{
    /// <summary>
    /// Provides URL validation. 
    /// </summary>
    internal class UrlDecorator : TypeScriptType
    {
        public UrlDecorator(string decoratorPath)
        {
            if (decoratorPath == null)
            {
                throw new ArgumentNullException(nameof(decoratorPath));
            }

            ModulePath = decoratorPath;
            Name = "Url";
        }

        public override string ToTypeScript() => "@Url()";
    }
}
