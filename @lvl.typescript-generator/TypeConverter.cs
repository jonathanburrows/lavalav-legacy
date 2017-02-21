using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

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
        /// <param name="packageForNamespace">The list of paths to npm modules, based on assembly.</param>
        /// <returns>The content of the converted typescript type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converting"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="packageForNamespace"/> is null.</exception>
        public TypeScriptType CsToTypeScript(Type converting, IReadOnlyDictionary<string, string> packageForNamespace)
        {
            if (converting == null)
            {
                throw new ArgumentNullException(nameof(converting));
            }
            if (packageForNamespace == null)
            {
                throw new ArgumentNullException(nameof(packageForNamespace));
            }

            var tsType = converting.IsInterface ? (TypeScriptType)new TypeScriptInterface() : new TypeScriptClass();
            tsType.Name = converting.Name;

            if (converting.BaseType != typeof(object))
            {
                tsType.BaseType = ConvertImportedType(converting.BaseType, packageForNamespace);
            }

            tsType.Interfaces = converting.GetInterfaces().Select(i => ConvertImportedType(i, packageForNamespace));
            tsType.Properties = converting.GetProperties().Select(p => ConvertPropertyInfo(p, packageForNamespace)).ToList();
            tsType.IsVisible = converting.IsVisible;

            return tsType;
        }

        /// <summary>
        /// Converts a property of a type, including any metadata.
        /// </summary>
        /// <param name="propertyInfo">The property to be converted.</param>
        /// <param name="packageForNamespace">The mapping of namespace to package directory.</param>
        /// <returns>The converted property.</returns>
        private TypeScriptProperty ConvertPropertyInfo(PropertyInfo propertyInfo, IReadOnlyDictionary<string, string> packageForNamespace)
        {
            var compareDecorators = propertyInfo
                .GetCustomAttributes<CompareAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "Compare", Arguments = new[] { v.OtherProperty } });
            var creditCardDecorators = propertyInfo
                .GetCustomAttributes<CreditCardAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "CreditCard" });
            var emailAddressDecorators = propertyInfo
                .GetCustomAttributes<EmailAddressAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "EmailAddress" });
            var maxLengthDecorators = propertyInfo
                .GetCustomAttributes<MaxLengthAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "MaxLength", Arguments = new object[] { v.Length } });
            var minLengthDecorators = propertyInfo
                .GetCustomAttributes<MinLengthAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "MinLength", Arguments = new object[] { v.Length } });
            var phoneDecorators = propertyInfo
                .GetCustomAttributes<PhoneAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "Phone" });
            var urlDecorators = propertyInfo
                .GetCustomAttributes<UrlAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "Url" });
            var rangeDecorators = propertyInfo
                .GetCustomAttributes<RangeAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "Range", Arguments = new object[] { v.Minimum, v.Maximum } });
            var regularExpressionDecorators = propertyInfo
                .GetCustomAttributes<RegularExpressionAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "RegularExpression", Arguments = new[] { v.Pattern } });
            var requiredDecorators = propertyInfo
                .GetCustomAttributes<RequiredAttribute>()
                .Select(v => new TypeScriptDecorator { Name = "Required" });
            var decorators = compareDecorators
                .Union(creditCardDecorators)
                .Union(emailAddressDecorators)
                .Union(maxLengthDecorators)
                .Union(minLengthDecorators)
                .Union(phoneDecorators)
                .Union(urlDecorators)
                .Union(rangeDecorators)
                .Union(regularExpressionDecorators)
                .Union(requiredDecorators);

            var propertyType = ConvertImportedType(propertyInfo.PropertyType, packageForNamespace);

            return new TypeScriptProperty
            {
                Name = propertyInfo.Name,
                Decorators = decorators,
                IsAbstract = propertyInfo.GetSetMethod().IsAbstract,
                IsOptional = !propertyInfo.PropertyType.IsValueType,
                PropertyType = propertyType
            };
        }

        /// <summary>
        /// Does a simple, shallow typescript conversion.
        /// </summary>
        /// <param name="imported">The type which will have a shallow conversion done.</param>
        /// <returns>The shallow converted type which can be used for imports.</returns>
        private TypeScriptType ConvertImportedType(Type imported, IReadOnlyDictionary<string, string> packageForNamespace)
        {
            var isPrimitive = imported == typeof(string) || imported.IsValueType;

            if (isPrimitive)
            {
                return new TypeScriptClass { Name = imported.Name };
            }
            else if (packageForNamespace.ContainsKey(imported.Namespace))
            {
                return new TypeScriptClass
                {
                    Name = imported.Name,
                    ModulePath = packageForNamespace[imported.Namespace]
                };
            }
            else
            {
                // consider it as a type within the generated npm package.
                return new TypeScriptClass { Name = imported.Name };
            }
        }
    }
}
