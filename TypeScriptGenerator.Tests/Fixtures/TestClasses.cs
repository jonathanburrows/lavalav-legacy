using System;
using System.ComponentModel.DataAnnotations;

// Due to the large amount of classes, they were centralized into one file here.
namespace lvl.TypeScriptGenerator.Tests.Fixtures
{
    public class BareClass { }

    public class BaseClass { }

    public class SingleInheritanceClass : BaseClass { }

    public interface IFirstInterface { }

    public class SingleInterfaceImplementationClass : IFirstInterface { }

    public interface ISecondInterface { }

    public class DoubleInterfaceImplementationClass : IFirstInterface, ISecondInterface { }

    public abstract class AbstractClass { }

    public class SimplePropertyClass
    {
        public int MyProperty { get; set; }
    }

    public class ComparePropertyClass
    {
        [Compare(nameof(OtherProperty))]
        public string Comparison { get; set; }
        public string OtherProperty { get; set; }
    }

    public class CreditCardClass
    {
        [CreditCard]
        public int CreditCardNumber { get; set; }
    }

    public class EmailAddressClass
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }

    public class MaxLengthClass
    {
        [MaxLength(1)]
        public string MaxLength { get; set; }
    }

    public class MinLengthClass
    {
        [MinLength(1)]
        public string MinLength { get; set; }
    }

    public class PhoneClass
    {
        [Phone]
        public string Phone { get; set; }
    }

    public class UrlClass
    {
        [Url]
        public string Url { get; set; }
    }

    public class RangeClass
    {
        [Range(1, 2)]
        public int Range { get; set; }
    }

    public class RegularExpressionClass
    {
        [RegularExpression("pattern")]
        public string RegularExpression { get; set; }
    }

    public class RequiredClass
    {
        [Required]
        public string Required { get; set; }
    }

    public class MultiAttributeClass
    {
        [Phone, Required]
        public int Phone { get; set; }
    }

    public class IntegerClass
    {
        public int Integer { get; set; }
    }

    public class NullableIntegerClass
    {
        public int? NullableInteger { get; set; }
    }

    public class DecimalClass
    {
        public decimal Decimal { get; set; }
    }

    public class NullableDecimalClass
    {
        public decimal? NullableDecimal { get; set; }
    }

    public class DoubleClass
    {
        public double Double { get; set; }
    }

    public class NullableDoubleClass
    {
        public double? NullableDouble { get; set; }
    }

    public class LongClass
    {
        public long Long { get; set; }
    }

    public class NullableLongClass
    {
        public long? NullableLong { get; set; }
    }

    public class StringClass
    {
        public string StringProperty { get; set; }
    }

    public class DateTimeClass
    {
        public DateTime DateTime { get; set; }
    }

    public class NullableDateTimeClass
    {
        public DateTime? NullableDateTime { get; set; }
    }

    public class BoolClass
    {
        public bool BoolProperty { get; set; }
    }

    public class NullableBoolClass
    {
        public bool? NullableBoolProperty { get; set; }
    }

    public class ChildClass { }

    public class SinglePropertyTypeClass
    {
        public ChildClass Child { get; set; }
    }

    public class SingleExternalImportClass
    {
        public ExternalLibrary.FirstClass FirstClass { get; set; }
    }

    public class DoubleExternalImportClass
    {
        public ExternalLibrary.FirstClass FirstClass { get; set; }
        public ExternalLibrary.SecondClass SecondClass { get; set; }
    }

    public class DoubleExternalImportDifferentNamespaceClass
    {
        public SecondExternalLibrary.FirstClass FirstClass { get; set; }
        public ExternalLibrary.SecondClass SecondClass { get; set; }
    }

    public class SameModuleInheritanceClass : BaseClass { }

    public class ExternalModuleInheritanceClass : ExternalLibrary.FirstClass { }

    public interface IBaseInterface { }

    public class SingleInterfaceSameNamespaceClass : IBaseInterface { }

    public class SingleInterfaceDifferentNamespaceClass : ExternalLibrary.IBaseInterface { }

    public class DoubleInterfaceDifferentNamespaceClass : SecondExternalLibrary.IFirstInterface, ExternalLibrary.ISecondInterface { }

    public class DoubleInterfaceFromSameExternalNamespaceClass : ExternalLibrary.IFirstInterface, ExternalLibrary.ISecondInterface { }

    public class ManglingTwoPropertiesClass
    {
        public ExternalLibrary.FirstClass FirstProperty { get; set; }
        public SecondExternalLibrary.FirstClass SecondProperty { get; set; }
    }

    public class ManglingPropertyAndBaseClass : SecondExternalLibrary.FirstClass
    {
        public ExternalLibrary.FirstClass FirstProperty { get; set; }
    }

    public class ManglingTwoInterfacesClass : ExternalLibrary.IFirstInterface, SecondExternalLibrary.IFirstInterface { }

    public interface IBareInterface { }

    public interface ISingleImplementationInterface : IBaseInterface { }

    public interface IDoubleImplementationInterface : IFirstInterface, ISecondInterface { }


    public interface IIntegerInterface
    {
        int Integer { get; set; }
    }

    public interface INullableIntegerInterface
    {
        int? NullableInteger { get; set; }
    }

    public interface IDecimalInterface
    {
        decimal Decimal { get; set; }
    }

    public interface INullableDecimalInterface
    {
        decimal? NullableDecimal { get; set; }
    }

    public interface IDoubleInterface
    {
        double Double { get; set; }
    }

    public interface INullableDoubleInterface
    {
        double? NullableDouble { get; set; }
    }

    public interface ILongInterface
    {
        long Long { get; set; }
    }

    public interface INullableLongInterface
    {
        long? NullableLong { get; set; }
    }

    public interface IStringInterface
    {
        string StringProperty { get; set; }
    }

    public interface IDateTimeInterface
    {
        DateTime DateTime { get; set; }
    }

    public interface INullableDateTimeInterface
    {
        DateTime? NullableDateTime { get; set; }
    }

    public interface IBoolInterface
    {
        bool BoolProperty { get; set; }
    }

    public interface INullableBoolInterface
    {
        bool? NullableBoolProperty { get; set; }
    }

    public interface IComplexPropertyInterface
    {
        BaseClass Property { get; set; }
    }

    public interface ISinglePropertySameNamespace
    {
        IBaseInterface Property { get; set; }
    }

    public interface ISinglePropertyDifferentNamespace
    {
        ExternalLibrary.IFirstInterface FirstProperty { get; set; }
    }

    public interface IDoublePropertySameNamespaceInterface
    {
        ExternalLibrary.IFirstInterface FirstProperty { get; set; }
        ExternalLibrary.ISecondInterface SecondProperty { get; set; }
    }

    public interface IDoublePropertyDifferentNamespacesInterface
    {
        SecondExternalLibrary.IFirstInterface FirstProperty { get; set; }
        ExternalLibrary.ISecondInterface SecondProperty { get; set; }
    }

    public interface IGenericInterfaceNoConstraints<TType> { }

    public interface IGenericInterfaceSingleConstraint<TBaseInterface> where TBaseInterface : IBareInterface { }

    public interface IGenericInterfaceDoubleConstraint<T> where T : BareClass, IBareInterface { }

    public interface IGenericDoubleInterface<TFirst, TSecond> { }

    public class GenericInterfaceImporter
    {
        public IGenericInterfaceNoConstraints<IBareInterface> Property { get; set; }
    }

    public class GenericClassNoConstraint<TType> { }

    public class GenericClassSingleConstraint<TBareClass> where TBareClass : BareClass { }

    public class GenericClassDoubleConstraint<T> where T : BareClass, IBareInterface { }

    public class GenericClassDouble<TFirst, TSecond> { }

    public class GenericClassImporter
    {
        public GenericClassNoConstraint<BareClass> Property { get; set; }
    }
}

namespace lvl.ExternalLibrary
{
    public interface IBaseInterface { }
    public interface IFirstInterface { }
    public interface ISecondInterface { }
    public class FirstClass { }
    public class SecondClass { }
}

namespace lvl.SecondExternalLibrary
{
    public interface IFirstInterface { }
    public class FirstClass { }
}