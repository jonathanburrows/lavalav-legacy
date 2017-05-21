using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    ///     Represets a type in TypeScript.
    /// </summary>
    public abstract class TypeScriptType
    {
        /// <summary>The name of the type.</summary>
        public string Name { get; set; }

        /// <summary>Name to use to prevent naming collisions.</summary>
        public string Alias { get; set; }

        /// <summary>Represents the next class in the inheritance chain</summary>
        public TypeScriptType BaseType { get; set; }

        /// <summary>Represents all the interfaces the type implements.</summary>
        public IList<TypeScriptType> Interfaces { get; set; }

        /// <summary>Represents the path to the type's module.</summary>
        public string ModulePath { get; set; }

        /// <summary>Denotes if the type should be exported from the module.</summary>
        public bool IsVisible { get; set; }

        /// <summary>Represents the accessible properties for the type.</summary>
        public IList<TypeScriptProperty> Properties { get; set; }

        /// <summary>Represents generic types.</summary>
        public IList<TypeScriptType> GenericArguments { get; set; }

        /// <summary>Represents constraints on the generic type.</summary>
        /// <remarks>This means that this type is a generic argument. </remarks>
        public IList<TypeScriptType> GenericConstraints { get; set; }

        /// <summary>Denotes if this is a global primative type.</summary>
        public bool IsPrimitive { get; set; }

        /// <summary>Denotes if this should be an array type.</summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// Will construct the necissary import statements.
        /// </summary>
        /// <returns>The typescript for importing the dependant libraries.</returns>
        protected string GetImportStatements()
        {
            MangleImports();

            var dependencies = GetDependencies();
            if (!dependencies.Any())
            {
                //we dont want any newlines at the top of the file if there's no dependencies.
                return string.Empty;
            }

            var importStatements = dependencies.GroupBy(d => d.ModulePath).Select(moduleDependencies =>
            {
                var modulePath = moduleDependencies.Key;

                var references = moduleDependencies
                    .Select(d => d.Alias != null ? $"{d.Name} as {d.Alias}" : $"{d.Name}")
                    .Distinct()
                    .OrderBy(r => r);
                var referencesStatement = string.Join(", ", references);

                return $"import {{ {referencesStatement} }} from '{modulePath}';";
            });

            return string.Join(Environment.NewLine, importStatements) + Environment.NewLine + Environment.NewLine;
        }

        /// <summary>
        /// Will resolve naming collisions by setting aliases.
        /// </summary>
        private void MangleImports()
        {
            var dependencies = GetDependencies();
            var dependenciesByName = dependencies.GroupBy(d => d.Name);
            foreach (var dependenciesWithName in dependenciesByName)
            {
                var name = dependenciesWithName.Key;

                var likeDependencies = dependenciesWithName.GroupBy(ld => ld.ModulePath).ToArray();

                for (var i = 1; i < likeDependencies.Length; i++)
                {
                    foreach (var likeDependency in likeDependencies[i])
                    {
                        likeDependency.Alias = $"{name}_{i}";
                    }
                }
            }
        }

        /// <summary>
        /// Will return all types which need to be imported.
        /// </summary>
        /// <returns>The types to be imported, imported by module path</returns>
        private IOrderedEnumerable<TypeScriptType> GetDependencies()
        {
            if (Properties == null)
            {
                throw new InvalidOperationException($"{nameof(Properties)} has not been properly initialized.");
            }
            if (Interfaces == null)
            {
                throw new InvalidOperationException($"{nameof(Interfaces)} has not been properly initialized.");
            }

            var propertyTypes = Properties.Select(p => p.PropertyType).Where(p => !p.IsPrimitive);

            var decoratorTypes = Properties.SelectMany(p => p.Decorators);

            var constraintTypes = GenericArguments.SelectMany(g => g.GenericConstraints);

            var argumentTypes = Interfaces.SelectMany(i => i.GenericArguments);

            var dependencies = propertyTypes.Union(Interfaces).Union(decoratorTypes).Union(constraintTypes).Union(argumentTypes);

            if (BaseType != null)
            {
                return dependencies.Union(new[] { BaseType }).OrderBy(d => d.ModulePath);
            }
            else
            {
                return dependencies.OrderBy(d => d.ModulePath);
            }
        }

        /// <summary>
        /// Will construct the necissary statements for interface implementation.
        /// </summary>
        /// <returns>The typescript for implementing interfaces.</returns>
        protected string GetImplementationStatements()
        {
            if (!Interfaces.Any())
            {
                return string.Empty;
            }

            var interfaces = string.Join(", ", Interfaces.Select(i =>
            {
                var mangledName = i.Alias ?? i.Name;
                return $"{mangledName}{i.GetGenericArgumentStatement()}";
            }));
            return $"implements {interfaces} ";
        }

        protected string GetGenericArgumentStatement()
        {
            if (!GenericArguments.Any())
            {
                return string.Empty;
            }
            else
            {
                var names = GenericArguments.Select(ga => ga.Name);
                return $"<{string.Join(", ", names)}>";
            }
        }

        protected string GetGenericConstraintStatement()
        {
            if (!GenericArguments.Any())
            {
                return string.Empty;
            }
            else
            {
                var statements = GenericArguments.Select(ga =>
                {
                    var constraints = ga.GenericConstraints.Select(c => c.Name).ToList();
                    var extends = constraints.Any() ? $" extends {string.Join(", ", constraints)}" : string.Empty;
                    return $"{ga.Name}{extends}";
                });
                return $"<{string.Join(", ", statements)}>";
            }
        }

        /// <summary>
        /// Will return a string which contains the code for a typescript type.
        /// </summary>
        /// <returns>The contents of the typescript type.</returns>
        public abstract string ToTypeScript();
    }
}
