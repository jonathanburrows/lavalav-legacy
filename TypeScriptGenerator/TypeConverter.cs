using lvl.TypescriptGenerator.Decorators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using lvl.TypescriptGenerator.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace lvl.TypescriptGenerator
{
    /// <summary>
    /// Converts types from C# to TypeScript.
    /// </summary>
    public class TypeConverter
    {
        /// <summary>
        /// Constructs a typescript object which is equivilant the given C# type.
        /// </summary>
        /// <param name="converting">The type to be converted to typescript.</param>
        /// <param name="generationOptions">The options to generate the typescript files.</param>
        /// <returns>The content of the converted typescript type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converting"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="generationOptions"/> is null.</exception>
        public TypeScriptType CsToTypeScript(Type converting, TypeScriptGenerationOptions generationOptions)
        {
            if (converting == null)
            {
                throw new ArgumentNullException(nameof(converting));
            }
            if (generationOptions == null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }
            if (generationOptions.PackageForNamespace == null)
            {
                throw new ArgumentNullException(nameof(generationOptions.PackageForNamespace));
            }

            var tsType = converting.IsInterface ? (TypeScriptType)new TypeScriptInterface() : new TypeScriptClass { IsAbstract = converting.IsAbstract };
            tsType.Name = GetTypeName(converting);

            if (converting.BaseType != typeof(object) && converting.BaseType != null)
            {
                tsType.BaseType = ConvertImportedType(converting.BaseType, generationOptions);
            }

            tsType.Interfaces = converting.GetInterfaces().Select(i => ConvertImportedType(i, generationOptions)).ToList();
            tsType.Properties = converting.GetProperties().Select(p => ConvertPropertyInfo(p, generationOptions)).ToList();
            tsType.GenericArguments = converting.GetGenericArguments().Select(p => ConvertGenericArgument(p, generationOptions)).ToList();
            tsType.IsVisible = converting.IsVisible;

            return tsType;
        }

        /// <summary>
        /// Converts a property of a type, including any metadata.
        /// </summary>
        /// <param name="propertyInfo">The property to be converted.</param>
        /// <param name="generationOptions">The mapping of namespace to package directory.</param>
        /// <returns>The converted property.</returns>
        private TypeScriptProperty ConvertPropertyInfo(PropertyInfo propertyInfo, TypeScriptGenerationOptions generationOptions)
        {
            var decoratorBin = generationOptions.DecoratorPath;

            // sorry for the repeatedness, the more clever solutions were harder to read.
            IEnumerable<TypeScriptType> compareDecorators = propertyInfo
                .GetCustomAttributes<CompareAttribute>()
                .Select(c => new CompareDecorator(decoratorBin, c));

            IEnumerable<TypeScriptType> creditCardDecorators = propertyInfo
                .GetCustomAttributes<CreditCardAttribute>()
                .Select(c => new CreditCardDecorator(decoratorBin));

            IEnumerable<TypeScriptType> emailAddressDecorators = propertyInfo
                .GetCustomAttributes<EmailAddressAttribute>()
                .Select(e => new EmailAddressDecorator(decoratorBin));

            IEnumerable<TypeScriptType> maxLengthDecorators = propertyInfo
                .GetCustomAttributes<MaxLengthAttribute>()
                .Select(m => new MaxLengthDecorator(decoratorBin, m));

            IEnumerable<TypeScriptType> minLengthDecorators = propertyInfo
                .GetCustomAttributes<MinLengthAttribute>()
                .Select(m => new MinLengthDecorator(decoratorBin, m));

            IEnumerable<TypeScriptType> phoneDecorators = propertyInfo
                .GetCustomAttributes<PhoneAttribute>()
                .Select(p => new PhoneDecorator(decoratorBin));

            IEnumerable<TypeScriptType> rangeDecorators = propertyInfo
                .GetCustomAttributes<RangeAttribute>()
                .Select(r => new RangeDecorator(decoratorBin, r));

            IEnumerable<TypeScriptType> regularExpressionDecorators = propertyInfo
                .GetCustomAttributes<RegularExpressionAttribute>()
                .Select(r => new RegularExpressionDecorator(decoratorBin, r));

            IEnumerable<TypeScriptType> requiredDecorators = propertyInfo
                .GetCustomAttributes<RequiredAttribute>()
                .Select(r => new RequiredDecorator(decoratorBin));

            IEnumerable<TypeScriptType> urlDecorators = propertyInfo
                .GetCustomAttributes<UrlAttribute>()
                .Select(u => new UrlDecorator(decoratorBin));

            var decorators = compareDecorators
                .Union(creditCardDecorators)
                .Union(emailAddressDecorators)
                .Union(maxLengthDecorators)
                .Union(minLengthDecorators)
                .Union(phoneDecorators)
                .Union(rangeDecorators)
                .Union(regularExpressionDecorators)
                .Union(requiredDecorators)
                .Union(urlDecorators);

            var propertyType = ConvertImportedType(propertyInfo.PropertyType, generationOptions);

            var isGeneric = propertyInfo.PropertyType.IsGenericType;
            var isOptional = isGeneric && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            return new TypeScriptProperty
            {
                Name = propertyInfo.Name,
                Decorators = decorators,
                IsAbstract = propertyInfo.GetSetMethod().IsAbstract,
                IsOptional = isOptional,
                PropertyType = propertyType
            };
        }

        /// <summary>
        /// Does a simple, shallow typescript conversion.
        /// </summary>
        /// <param name="imported">The type which will have a shallow conversion done.</param>
        /// <param name="generationOptions">The options, including mapping to different npm packages.</param>
        /// <returns>The shallow converted type which can be used for imports.</returns>
        private TypeScriptType ConvertImportedType(Type imported, TypeScriptGenerationOptions generationOptions)
        {
            if (imported.IsArray)
            {
                var type = imported.GetElementType();
                return new TypeScriptClass
                {
                    Name = GetTypeName(type),
                    ModulePath = GetPathForType(type, generationOptions),
                    IsPrimitive = type == typeof(string) || type.IsValueType,
                    IsCollection = true
                };
            }
            else if (typeof(IEnumerable).IsAssignableFrom(imported) && imported != typeof(string))
            {
                var type = imported.GetGenericArguments().Single();
                return new TypeScriptClass
                {
                    Name = GetTypeName(type),
                    ModulePath = GetPathForType(type, generationOptions),
                    IsPrimitive = type == typeof(string) || type.IsValueType,
                    IsCollection = true
                };
            }
            else
            {
                return new TypeScriptClass
                {
                    Name = GetTypeName(imported),
                    ModulePath = GetPathForType(imported, generationOptions),
                    IsPrimitive = imported == typeof(string) || imported.IsValueType
                };
            }
        }

        /// <summary>
        ///     Provides information on generic type parameters and constraints.
        /// </summary>
        /// <param name="genericArgument">The generic argument type.</param>
        /// <param name="generationOptions">The options, including mapping to different npm packages.</param>
        /// <returns>The converted type for the generic argument.</returns>
        private TypeScriptType ConvertGenericArgument(Type genericArgument, TypeScriptGenerationOptions generationOptions)
        {
            if (!genericArgument.IsGenericParameter)
            {
                throw new InvalidOperationException("Type is not a generic parameter");
            }
            var constraints = genericArgument.GetGenericParameterConstraints().Select(constraint => ConvertImportedType(constraint, generationOptions)).ToList();

            return new TypeScriptClass
            {
                Name = genericArgument.Name,
                GenericConstraints = constraints
            };
        }

        private string GetPathForType(Type type, TypeScriptGenerationOptions generationOptions)
        {
            var packageForNamespace = generationOptions.PackageForNamespace;
            if (type == typeof(string) || type.IsValueType)
            {
                return null;
            }
            else if (packageForNamespace.ContainsKey(type.Namespace))
            {
                return packageForNamespace[type.Namespace];
            }
            else
            {
                var friendlyName = type.Name.Split('`')[0];
                return $"./{friendlyName.ToDashed()}";
            }
        }

        private string GetTypeName(Type type)
        {
            var numberTypes = new[] { typeof(int), typeof(int?), typeof(double), typeof(double?), typeof(decimal), typeof(decimal?), typeof(long), typeof(long?) };
            if (numberTypes.Any(t => t.IsAssignableFrom(type)))
            {
                return "number";
            }

            var dateTypes = new[] { typeof(DateTime), typeof(DateTime?) };
            if (dateTypes.Any(t => t.IsAssignableFrom(type)))
            {
                return "Date";
            }

            var booleanTypes = new[] { typeof(bool), typeof(bool?) };
            if (booleanTypes.Any(t => t.IsAssignableFrom(type)))
            {
                return "boolean";
            }

            if (typeof(string).IsAssignableFrom(type))
            {
                return "string";
            }

            if (type.IsGenericType)
            {
                return type.Name.Split('`')[0];
            }

            return type.Name;
        }
    }
}
