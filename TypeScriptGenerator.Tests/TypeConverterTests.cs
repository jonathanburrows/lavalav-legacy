using lvl.TypescriptGenerator;
using lvl.TypeScriptGenerator.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
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
        private TypeScriptGenerationOptions GenerationOptions { get; }

        public TypeConverterTests()
        {
            TypeConverter = new TypeConverter();
            GenerationOptions = new TypeScriptGenerationOptions
            {
                DecoratorPath = "@lvl/front-end",
                PackageForNamespace = new Dictionary<string, string>
                {
                    ["lvl.ExternalLibrary"] = "@lvl/external-library",
                    ["lvl.SecondExternalLibrary"] = "@lvl/second-external-library",
                }
            };
        }

        [Fact]
        public void Converting_cs_type_will_throw_argument_null_exception_when_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(null, GenerationOptions));
        }

        [Fact]
        public void Converting_cs_type_will_throw_argument_null_exception_when_options_are_null()
        {
            var type = GetType();

            Assert.Throws<ArgumentNullException>(() => TypeConverter.CsToTypeScript(type, null));
        }

        [Fact]
        public void It_will_export_class_name()
        {
            var type = typeof(BareClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class {type.Name} {{", tsContent);
        }

        [Fact]
        public void It_will_have_extend_when_class_has_single_inheritance()
        {
            var type = typeof(SingleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class SingleInheritanceClass extends BaseClass {", tsContent);
        }

        [Fact]
        public void It_will_have_implements_when_class_has_single_interface()
        {
            var type = typeof(SingleInterfaceImplementationClass);
            var interfaceType = type.GetInterfaces().Single();

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export class {type.Name} implements {interfaceType.Name} {{", tsContent);
        }

        [Fact]
        public void It_will_have_implements_with_comma_delimited_list_when_class_has_two_interfaces()
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
        public void It_will_have_abstract_keyword_when_class_is_abstract()
        {
            var type = typeof(AbstractClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"export abstract class {type.Name} {{", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_public_for_each_property()
        {
            var type = typeof(SimplePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Matches(@"public .*\:", tsContent);
        }

        [Fact]
        public void It_will_write_properties_with_pascal_names()
        {
            var type = typeof(SimplePropertyClass);
            var propertyName = "myProperty";

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"public {propertyName}:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_compare_when_property_has_compare_decorator()
        {
            var type = typeof(ComparePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Compare('otherProperty') public comparison:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_credit_card_when_property_has_credit_card_decorator()
        {
            var type = typeof(CreditCardClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@CreditCard() public creditCardNumber:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_email_when_property_has_email_decorator()
        {
            var type = typeof(EmailAddressClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@EmailAddress() public emailAddress:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_when_max_length_when_property_has_max_length_decorator()
        {
            var type = typeof(MaxLengthClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@MaxLength(1) public maxLength:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_min_length_when_property_has_min_length_decorator()
        {
            var type = typeof(MinLengthClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@MinLength(1) public minLength:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_phone_when_property_has_phone_decorator()
        {
            var type = typeof(PhoneClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Phone() public phone:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_url_when_property_has_url_decorator()
        {
            var type = typeof(UrlClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Url() public url:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_range_when_property_has_url()
        {
            var type = typeof(RangeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Range(1, 2) public range:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_regular_expression_when_property_has_regular_expression_decorator()
        {
            var type = typeof(RegularExpressionClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@RegularExpression(/pattern/) public regularExpression:", tsContent);
        }

        [Fact]
        public void It_will_prefix_properties_with_required_when_property_has_required_decorator()
        {
            var type = typeof(RequiredClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Required() public required:", tsContent);
        }

        [Fact]
        public void It_will_property_with_comma_seperated_annotations_when_property_has_multiple_validation_decorators()
        {
            var type = typeof(MultiAttributeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("@Phone(), @Required() public phone:", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_integer()
        {
            var type = typeof(IntegerClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public integer: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_nullable_integer()
        {
            var type = typeof(NullableIntegerClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableInteger: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_decimal()
        {
            var type = typeof(DecimalClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public decimal: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_nullable_decimal()
        {
            var type = typeof(NullableDecimalClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDecimal: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_double()
        {
            var type = typeof(DoubleClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public double: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_nullable_double()
        {
            var type = typeof(NullableDoubleClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDouble: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_long()
        {
            var type = typeof(LongClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public long: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_number_when_property_is_nullable_long()
        {
            var type = typeof(NullableLongClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableLong: number", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_string_when_property_is_string()
        {
            var type = typeof(StringClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public stringProperty: string", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_date_when_property_is_date_time()
        {
            var type = typeof(DateTimeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public dateTime: Date", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_date_when_property_is_nullable_date_time()
        {
            var type = typeof(NullableDateTimeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableDateTime: Date", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_boolean_when_property_is_bool()
        {
            var type = typeof(BoolClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public boolProperty: boolean", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_boolean_when_property_is_nullable_bool()
        {
            var type = typeof(NullableBoolClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public nullableBoolProperty: boolean", tsContent);
        }

        [Fact]
        public void It_will_suffix_property_with_type_name_when_property_is_entity_type()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("public child: ChildClass", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_for_property_when_from_same_assembly()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("import { ChildClass } from './child-class';", tsContent);
        }

        [Fact]
        public void It_will_generate_external_import_statement_for_property_from_differnt_assembly()
        {
            var type = typeof(SingleExternalImportClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_declare_two_types_in_import_statement_for_properties_of_type_from_same_external_namespace()
        {
            var type = typeof(DoubleExternalImportClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass, SecondClass }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_declare_two_import_statements_for_properties_of_type_from_different_external_namespace()
        {
            var type = typeof(DoubleExternalImportDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ SecondClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_generate_import_for_same_namespace()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { ChildClass } from './child-class';", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_when_class_inherits_from_class_in_same_namespace()
        {
            var type = typeof(SameModuleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { BaseClass } from './base-class';", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_when_class_inherits_from_class_in_differnt_assembly()
        {
            var type = typeof(ExternalModuleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { FirstClass } from '@lvl/external-library';", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_when_class_implements_interface_from_same_namespace()
        {
            var type = typeof(SingleInterfaceSameNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { IBaseInterface } from './ibase-interface';", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_when_class_implements_interface_from_different_assembly()
        {
            var type = typeof(SingleInterfaceDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith("import { IBaseInterface } from '@lvl/external-library';", tsContent);
        }

        [Fact]
        public void It_will_generate_two_import_statements_when_class_implements_two_interfaces_from_different_assembly_with_different_namespaces()
        {
            var type = typeof(DoubleInterfaceDifferentNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ ISecondInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_generate_single_import_with_two_declarations_when_class_implements_two_interfaces_from_single_external_assembly_namespace()
        {
            var type = typeof(DoubleInterfaceFromSameExternalNamespaceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface, ISecondInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_mangle_type_import_name_when_class_has_property_types_with_same()
        {
            var type = typeof(ManglingTwoPropertiesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass as FirstClass_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_mangle_property_type_name_when_class_has_property_types_with_same()
        {
            var type = typeof(ManglingTwoPropertiesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"public firstProperty: FirstClass;{NewLine}    public secondProperty: FirstClass_1;", tsContent);
        }

        [Fact]
        public void It_will_mangle_type_import_name_when_class_has_property_and_interface_with_same_type_name()
        {
            var type = typeof(ManglingPropertyAndBaseClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ FirstClass }} from '@lvl/external-library';{NewLine}import {{ FirstClass as FirstClass_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_mangle_interface_name_when_class_has_property_and_interface_with_same_type_name()
        {
            var type = typeof(ManglingPropertyAndBaseClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Matches("export class ManglingPropertyAndBaseClass extends FirstClass_1 {", tsContent);
        }

        [Fact]
        public void It_will_mangle_type_import_when_class_implements_two_interfaces_wth_same_name()
        {
            var type = typeof(ManglingTwoInterfacesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface as IFirstInterface_1 }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_interface_names_when_class_implements_two_interfaces_wth_same_name()
        {
            var type = typeof(ManglingTwoInterfacesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class ManglingTwoInterfacesClass implements IFirstInterface, IFirstInterface_1 {", tsContent);
        }

        [Fact]
        public void It_will_export_interface_name()
        {
            var type = typeof(IBareInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IBareInterface {", tsContent);
        }

        [Fact]
        public void It_will_generate_implement_statement_when_interface_implements_another_interface()
        {
            var type = typeof(ISingleImplementationInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface ISingleImplementationInterface implements IBaseInterface {", tsContent);
        }

        [Fact]
        public void It_will_generate_implement_with_comma_seperate_interface_names_when_interface_implements_two_interfaces()
        {
            var type = typeof(IDoubleImplementationInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IDoubleImplementationInterface implements IFirstInterface, ISecondInterface {", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_number_when_property_type_is_int()
        {
            var type = typeof(IIntegerInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("integer: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_number_when_property_type_is_nullable_int()
        {
            var type = typeof(INullableIntegerInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableInteger?: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_number_when_property_type_is_decimal()
        {
            var type = typeof(IDecimalInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("decimal: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_number_when_property_type_is_nullable_decimal()
        {
            var type = typeof(INullableDecimalInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDecimal?: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_number_when_property_type_is_double()
        {
            var type = typeof(IDoubleInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("double: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_when_property_type_is_nullable_double()
        {
            var type = typeof(INullableDoubleInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDouble?: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_number_when_property_type_is_long()
        {
            var type = typeof(ILongInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("long: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_number_when_property_type_is_nullable_long()
        {
            var type = typeof(INullableLongInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableLong?: number;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_string_when_property_type_is_string()
        {
            var type = typeof(IStringInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("stringProperty: string;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_date_when_property_type_is_date_time()
        {
            var type = typeof(IDateTimeInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("dateTime: Date;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_date_when_property_type_is_nullable_date_time()
        {
            var type = typeof(INullableDateTimeInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableDateTime?: Date;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_boolean_when_property_type_is_bool()
        {
            var type = typeof(IBoolInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("boolProperty: boolean;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_optional_boolean_when_property_type_is_nullable_bool()
        {
            var type = typeof(INullableBoolInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("nullableBoolProperty?: boolean;", tsContent);
        }

        [Fact]
        public void It_will_suffix_interface_property_with_type_name_when_property_type_is_reference_to_entity()
        {
            var type = typeof(IComplexPropertyInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("property: BaseClass;", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_for_interface_property_type_when_type_is_from_same_assembly()
        {
            var type = typeof(ISinglePropertySameNamespace);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IBaseInterface }} from './ibase-interface';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_generate_import_statement_for_interface_type_when_type_is_from_referenced_assembly()
        {
            var type = typeof(ISinglePropertyDifferentNamespace);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_generate_two_declaration_statements_for_interface_property_types_when_properties_are_from_same_referenced_namespaces()
        {
            var type = typeof(IDoublePropertySameNamespaceInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IFirstInterface, ISecondInterface }} from '@lvl/external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_generate_two_import_statements_for_interface_property_types_when_properties_are_from_different_referenced_namespaces()
        {
            var type = typeof(IDoublePropertyDifferentNamespacesInterface);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ ISecondInterface }} from '@lvl/external-library';{NewLine}import {{ IFirstInterface }} from '@lvl/second-external-library';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Generic_interface_has_brackets_containing_type()
        {
            var type = typeof(IGenericInterfaceNoConstraints<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IGenericInterfaceNoConstraints<TType> {", tsContent);
        }

        [Fact]
        public void Generic_interface_with_constraint_shows_with_extends()
        {
            var type = typeof(IGenericInterfaceSingleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IGenericInterfaceSingleConstraint<TBaseInterface extends IBareInterface> {", tsContent);
        }

        [Fact]
        public void Generic_interface_with_constraint_will_import_constraint()
        {
            var type = typeof(IGenericInterfaceSingleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ IBareInterface }} from './ibare-interface';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Generic_interface_with_two_constraints_shows_in_extends()
        {
            var type = typeof(IGenericInterfaceDoubleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IGenericInterfaceDoubleConstraint<T extends BareClass, IBareInterface> {", tsContent);
        }

        [Fact]
        public void Generic_interface_with_two_types_shows_both()
        {
            var type = typeof(IGenericDoubleInterface<,>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export interface IGenericDoubleInterface<TFirst, TSecond> {", tsContent);
        }

        [Fact]
        public void Generic_interface_being_imported_will_not_include_generic_parameter()
        {
            var type = typeof(GenericInterfaceImporter);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"import {{ IGenericInterfaceNoConstraints }} from './igeneric-interface-no-constraints';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Generic_class_has_brackets_containing_type()
        {
            var type = typeof(GenericClassNoConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class GenericClassNoConstraint<TType> {", tsContent);
        }

        [Fact]
        public void Generic_class_with_constraint_shows_with_extends()
        {
            var type = typeof(GenericClassSingleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class GenericClassSingleConstraint<TBareClass extends BareClass> {", tsContent);
        }

        [Fact]
        public void Generic_class_with_constraint_will_import_constraint()
        {
            var type = typeof(GenericClassSingleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.StartsWith($"import {{ BareClass }} from './bare-class';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void Generic_class_with_two_constraints_shows_in_extends()
        {
            var type = typeof(GenericClassDoubleConstraint<>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class GenericClassDoubleConstraint<T extends BareClass, IBareInterface> {", tsContent);
        }

        [Fact]
        public void Generic_class_with_two_types_shows_both()
        {
            var type = typeof(GenericClassDouble<,>);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("export class GenericClassDouble<TFirst, TSecond> {", tsContent);
        }

        [Fact]
        public void Generic_class_being_imported_will_not_include_generic_parameter()
        {
            var type = typeof(GenericClassImporter);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains($"import {{ GenericClassNoConstraint }} from './generic-class-no-constraint';{NewLine}{NewLine}", tsContent);
        }

        [Fact]
        public void It_will_not_render_super_when_base_class()
        {
            var type = typeof(BaseClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.DoesNotMatch("super(.*);", tsContent);
        }

        [Fact]
        public void It_will_render_super_for_derived_class()
        {
            var type = typeof(SameModuleInheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Matches("super(.*);", tsContent);
        }

        [Fact]
        public void It_will_render_an_options_interface()
        {
            var type = typeof(SimplePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("interface ISimplePropertyClassOptions {", tsContent);
        }

        [Fact]
        public void It_will_have_an_options_property_in_pascal()
        {
            var type = typeof(SimplePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("myProperty?:", tsContent);
        }

        [Fact]
        public void It_will_have_a_options_property_with_a_primitive_property_type()
        {
            var type = typeof(SimplePropertyClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("myProperty?: number;", tsContent);
        }

        [Fact]
        public void It_will_have_a_options_property_with_a_complex_property_type()
        {
            var type = typeof(SinglePropertyTypeClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("child?: ChildClass;", tsContent);
        }

        [Fact]
        public void It_will_render_an_array_for_an_options_party()
        {
            var type = typeof(ArrayClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("children?: number[];", tsContent);
        }

        [Fact]
        public void It_will_mangle_options_property_types()
        {
            var type = typeof(ManglingTwoPropertiesClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("secondProperty?: FirstClass_1;", tsContent);
        }

        [Fact]
        public void It_will_render_an_options_property_for_inherited_property()
        {
            var type = typeof(InheritanceClass);

            var tsType = TypeConverter.CsToTypeScript(type, GenerationOptions);
            var tsContent = tsType.ToTypeScript();

            Assert.Contains("myProperty?: number;", tsContent);
        }
    }
}
