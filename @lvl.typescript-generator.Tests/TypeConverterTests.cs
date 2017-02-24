using lvl.TypescriptGenerator;
using lvl.TypeScriptGenerator.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static System.Environment;

namespace lvl.TypeScriptGenerator.Tests
{
    public class TypeConverterTests
    {
        private TypeConverter TypeConverter { get; }
        private GenerationOptions GenerationOptions { get; }

        public TypeConverterTests()
        {
            TypeConverter = new TypeConverter();
            GenerationOptions = new GenerationOptions
            {
                DecoratorPath = "@lvl/core",
                PackageForNamespace = new Dictionary<string, string>
                {
                    ["lvl.ExternalLibrary"] = "@lvl/external-library",
                    ["lvl.SecondExternalLibrary"] = "@lvl/second-external-library",
                }
            };
        }

        [Fact]
        public void ConvertingNullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(null, GenerationOptions));
        }

        [Fact]
        public void ConvertingNullPackagesForNamespace_ThrowsArgumentNullException()
        {
            var type = GetType();

            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(type, null));
        }

        [Fact]
        public void Class_NameExported()
        {
            var type = typeof(BareClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class {type.Name} {{", tsContent);
        }

        [Fact]
        public void Class_SingleInheritance()
        {
            var type = typeof(SingleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class SingleInheritanceClass extends BaseClass {", tsContent);
        }

        [Fact]
        public void Class_SingleInterfaceImplementation()
        {
            var type = typeof(SingleInterfaceImplementationClass);
            var interfaceType = type.GetInterfaces().Single();

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class {type.Name} implements {interfaceType.Name} {{", tsContent);
        }

        [Fact]
        public void Class_DoubleInterfaceImplementation()
        {
            var type = typeof(DoubleInterfaceImplementationClass);
            var interfaces = type.GetInterfaces().ToList();
            var firstInterface = interfaces[0];
            var secondInterface = interfaces[1];

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class {type.Name} implements {firstInterface.Name}, {secondInterface.Name} {{", tsContent);
        }

        [Fact]
        public void Class_AbstractIdentifier()
        {
            var type = typeof(AbstractClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export abstract class {type.Name} {{", tsContent);
        }

        [Fact]
        public void Class_Property_IsPublic()
        {
            var type = typeof(SimplePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Matches(@"public .*\:", tsContent);
        }

        [Fact]
        public void Class_PropertyName_IsPascal()
        {
            var type = typeof(SimplePropertyClass);
            var propertyName = "myProperty";

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"public {propertyName}:", tsContent);
        }

        [Fact]
        public void Class_Property_CompareDecorator()
        {
            var type = typeof(ComparePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Compare('otherProperty') public comparison:", tsContent);
        }

        [Fact]
        public void Class_Property_CreditCardDecorator()
        {
            var type = typeof(CreditCardClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@CreditCard() public creditCardNumber:", tsContent);
        }

        [Fact]
        public void Class_Property_EmailDecorator()
        {
            var type = typeof(EmailAddressClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@EmailAddress() public emailAddress:", tsContent);
        }

        [Fact]
        public void Class_Property_MaxLengthDecorator()
        {
            var type = typeof(MaxLengthClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@MaxLength(1) public maxLength:", tsContent);
        }

        [Fact]
        public void Class_Property_MinLengthDecorator()
        {
            var type = typeof(MinLengthClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@MinLength(1) public minLength:", tsContent);
        }

        [Fact]
        public void Class_Property_PhoneDecorator()
        {
            var type = typeof(PhoneClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Phone() public phone:", tsContent);
        }

        [Fact]
        public void Class_Property_UrlDecorator()
        {
            var type = typeof(UrlClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Url() public url:", tsContent);
        }

        [Fact]
        public void Class_Property_RangeDecorator()
        {
            var type = typeof(RangeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Range(1, 2) public range:", tsContent);
        }

        [Fact]
        public void Class_Property_RegularExpressionDecorator()
        {
            var type = typeof(RegularExpressionClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@RegularExpression(/pattern/) public regularExpression:", tsContent);
        }

        [Fact]
        public void Class_Property_RequiredDecorator()
        {
            var type = typeof(RequiredClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Required() public required:", tsContent);
        }

        [Fact]
        public void Class_Property_MultipleDecorators()
        {
            var type = typeof(MultiAttributeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Phone(), @Required() public phone:", tsContent);
        }

        [Fact]
        public void Class_Property_Integer()
        {
            var type = typeof(IntegerClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public integer: number", tsContent);
        }

        [Fact]
        public void Class_Property_NullableInteger()
        {
            var type = typeof(NullableIntegerClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableInteger: number", tsContent);
        }

        [Fact]
        public void Class_Property_Decimal()
        {
            var type = typeof(DecimalClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public decimal: number", tsContent);
        }

        [Fact]
        public void Class_Property_NullableDecimal()
        {
            var type = typeof(NullableDecimalClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDecimal: number", tsContent);
        }

        [Fact]
        public void Class_Property_Double()
        {
            var type = typeof(DoubleClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public double: number", tsContent);
        }

        [Fact]
        public void Class_Property_NullableDouble()
        {
            var type = typeof(NullableDoubleClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDouble: number", tsContent);
        }

        [Fact]
        public void Class_Property_Long()
        {
            var type = typeof(LongClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public long: number", tsContent);
        }

        [Fact]
        public void Class_Property_NullableLong()
        {
            var type = typeof(NullableLongClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableLong: number", tsContent);
        }

        [Fact]
        public void Class_Property_String()
        {
            var type = typeof(StringClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public stringProperty: string", tsContent);
        }

        [Fact]
        public void Class_Property_DateTime()
        {
            var type = typeof(DateTimeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public dateTime: Date", tsContent);
        }

        [Fact]
        public void Class_Property_NullableDateTime()
        {
            var type = typeof(NullableDateTimeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDateTime: Date", tsContent);
        }

        [Fact]
        public void Class_Property_Bool()
        {
            var type = typeof(BoolClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public boolProperty: boolean", tsContent);
        }

        [Fact]
        public void Class_Property_NullableBool()
        {
            var type = typeof(NullableBoolClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableBoolProperty: boolean", tsContent);
        }

        [Fact]
        public void Class_Property_ComplexProperty()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public child: ChildClass", tsContent);
        }

        [Fact]
        public void Class_Import_Single()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public child: ChildClass", tsContent);
        }

        [Fact]
        public void Class_Import_SingleExternal()
        {
            var type = typeof(SingleExternalImportClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Import_TwoExternalSameNamespace()
        {
            var type = typeof(DoubleExternalImportClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass, SecondClass }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Import_TwoExternalDifferentNamespaces()
        {
            var type = typeof(DoubleExternalImportDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ SecondClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Import_SameNamespace()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { ChildClass } from './child-class';", tsContent);
        }

        [Fact]
        public void Class_Import_InheritanceFromSameNamespace()
        {
            var type = typeof(SameModuleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { BaseClass } from './base-class';", tsContent);
        }

        [Fact]
        public void Class_Import_InheritanceFromDifferentNamespace()
        {
            var type = typeof(ExternalModuleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { FirstClass } from '@lvl/external-library';", tsContent);
        }

        [Fact]
        public void Class_Import_SingleInterfaceSameNamespace()
        {
            var type = typeof(SingleInterfaceSameNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { IBaseInterface } from './ibase-interface';", tsContent);
        }

        [Fact]
        public void Class_Import_SingleInterfaceDifferentNamespace()
        {
            var type = typeof(SingleInterfaceDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { IBaseInterface } from '@lvl/external-library';", tsContent);
        }

        [Fact]
        public void Class_Import_DoubleInterfaceDifferentNamespace()
        {
            var type = typeof(DoubleInterfaceDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ ISecondInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Import_DoubleInterfaceFromSameButExternalNamespace()
        {
            var type = typeof(DoubleInterfaceFromSameExternalNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface, ISecondInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Mangling_TwoProperties_Imports()
        {
            var type = typeof(ManglingTwoPropertiesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass as FirstClass_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Mangling_TwoProperties_References()
        {
            var type = typeof(ManglingTwoPropertiesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"public firstProperty: FirstClass;{NewLine}    public secondProperty: FirstClass_1;", tsContent);
        }

        [Fact]
        public void Class_Mangling_PropertyAndInheritance_Imports()
        {
            var type = typeof(ManglingPropertyAndBaseClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass as FirstClass_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }
        
        [Fact]
        public void Class_Mangling_PropertyAndInheritance_References()
        {
            var type = typeof(ManglingPropertyAndBaseClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class ManglingPropertyAndBaseClass extends FirstClass_1 {{{NewLine}    public firstProperty: FirstClass;", tsContent);
        }

        [Fact]
        public void Class_Mangling_TwoInterfaces_Imports()
        {
            var type = typeof(ManglingTwoInterfacesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface as IFirstInterface_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Class_Mangling_TwoInterfaces_References()
        {
            var type = typeof(ManglingTwoInterfacesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class ManglingTwoInterfacesClass implements IFirstInterface, IFirstInterface_1 {", tsContent);
        }

        [Fact]
        public void Interface_Name()
        {
            var type = typeof(IBareInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IBareInterface {", tsContent);
        }

        [Fact]
        public void Interface_SingleImplementation()
        {
            var type = typeof(ISingleImplementationInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface ISingleImplementationInterface implements IBaseInterface {", tsContent);
        }

        [Fact]
        public void Interface_DoubleImplementation()
        {
            var type = typeof(IDoubleImplementationInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IDoubleImplementationInterface implements IFirstInterface, ISecondInterface {", tsContent);
        }

        [Fact]
        public void Interface_Property_Integer()
        {
            var type = typeof(IIntegerInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("integer: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableInteger()
        {
            var type = typeof(INullableIntegerInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableInteger?: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_Decimal()
        {
            var type = typeof(IDecimalInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("decimal: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableDecimal()
        {
            var type = typeof(INullableDecimalInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDecimal?: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_Double()
        {
            var type = typeof(IDoubleInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("double: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableDouble()
        {
            var type = typeof(INullableDoubleInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDouble?: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_Long()
        {
            var type = typeof(ILongInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("long: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableLong()
        {
            var type = typeof(INullableLongInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableLong?: number;", tsContent);
        }

        [Fact]
        public void Interface_Property_String()
        {
            var type = typeof(IStringInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("stringProperty: string;", tsContent);
        }

        [Fact]
        public void Interface_Property_DateTime()
        {
            var type = typeof(IDateTimeInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("dateTime: Date;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableDateTime()
        {
            var type = typeof(INullableDateTimeInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDateTime?: Date;", tsContent);
        }

        [Fact]
        public void Interface_Property_Bool()
        {
            var type = typeof(IBoolInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("boolProperty: boolean;", tsContent);
        }

        [Fact]
        public void Interface_Property_NullableBool()
        {
            var type = typeof(INullableBoolInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableBoolProperty?: boolean;", tsContent);
        }

        [Fact]
        public void Interface_Property_Complex()
        {
            var type = typeof(IComplexPropertyInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("property: BaseClass;", tsContent);
        }

        [Fact]
        public void Interface_Imports_SinglePropertySameNamespace()
        {
            var type = typeof(ISinglePropertySameNamespace);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IBaseInterface }} from './ibase-interface';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Interface_Imports_SinglePropertyDifferentNamespace()
        {
            var type = typeof(ISinglePropertyDifferentNamespace);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Interface_Imports_DoublePropertySameInterface()
        {
            var type = typeof(IDoublePropertySameNamespaceInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface, ISecondInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Interface_Imports_DoublePropertyDifferentInterfaces()
        {
            var type = typeof(IDoublePropertyDifferentNamespacesInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ ISecondInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }
    }
}
