namespace lvl.Web.OData.Tokens
{
    /// <summary>
    /// Multiple classes were put into once class due to each token being very thin.
    /// </summary>
    public abstract class Token
    {
        public string Value { get; set; }
        public abstract string Pattern { get; }
    }

    internal abstract class FunctionToken : Token { }

    internal abstract class ValueToken : Token { }

    internal abstract class UnaryOperatorToken : Token { }

    internal abstract class BinaryOperatorToken : Token { }

    internal class ConcatToken : FunctionToken
    {
        public override string Pattern => "concat";
    }

    internal class SubstringOfToken : FunctionToken
    {
        public override string Pattern => "substringof";
    }

    internal class EndsWithToken : FunctionToken
    {
        public override string Pattern => "endswith";
    }

    internal class StartsWithToken : FunctionToken
    {
        public override string Pattern => "startswith";
    }

    internal class LengthToken : FunctionToken
    {
        public override string Pattern => "length";
    }

    internal class IndexOfToken : FunctionToken
    {
        public override string Pattern => "indexof";
    }

    internal class ReplaceToken : FunctionToken
    {
        public override string Pattern => "replace";
    }

    internal class SubstringToken : FunctionToken
    {
        public override string Pattern => "substring";
    }

    internal class ToLowerToken : FunctionToken
    {
        public override string Pattern => "tolower";
    }

    internal class ToUpperToken : FunctionToken
    {
        public override string Pattern => "toupper";
    }

    internal class TrimToken : FunctionToken
    {
        public override string Pattern => "trim";
    }

    internal class DayToken : FunctionToken
    {
        public override string Pattern => "day";
    }

    internal class HourToken : FunctionToken
    {
        public override string Pattern => "hour";
    }

    internal class MinuteToken : FunctionToken
    {
        public override string Pattern => "minute";
    }

    internal class MonthToken : FunctionToken
    {
        public override string Pattern => "month";
    }

    internal class SecondToken : FunctionToken
    {
        public override string Pattern => "second";
    }

    internal class YearToken : FunctionToken
    {
        public override string Pattern => "year";
    }

    internal class RoundToken : FunctionToken
    {
        public override string Pattern => "round";
    }

    internal class FloorToken : FunctionToken
    {
        public override string Pattern => "floor";
    }

    internal class CeilngToken : FunctionToken
    {
        public override string Pattern => "ceiling";
    }

    internal class OpenBracketToken : Token
    {
        public override string Pattern => @"\(";
    }

    internal class CloseBracketToken : Token
    {
        public override string Pattern => @"\)";
    }

    internal class NotToken : UnaryOperatorToken
    {
        public override string Pattern => "not ";
    }

    internal class EqualsToken : BinaryOperatorToken
    {
        public override string Pattern => "eq ";
    }

    internal class NotEqualsToken : BinaryOperatorToken
    {
        public override string Pattern => "ne ";
    }

    internal class GreaterThanToken : BinaryOperatorToken
    {
        public override string Pattern => "gt ";
    }

    internal class GreaterThanEqualToken : BinaryOperatorToken
    {
        public override string Pattern => "ge ";
    }

    internal class LessThanToken : BinaryOperatorToken
    {
        public override string Pattern => "lt ";
    }

    internal class LessThanEqualToken : BinaryOperatorToken
    {
        public override string Pattern => "le ";
    }

    internal class AndToken : BinaryOperatorToken
    {
        public override string Pattern => "and ";
    }

    internal class OrToken : BinaryOperatorToken
    {
        public override string Pattern => "or ";
    }

    internal class AdditionToken : BinaryOperatorToken
    {
        public override string Pattern => "add ";
    }

    internal class SubtractionToken : BinaryOperatorToken
    {
        public override string Pattern => "sub ";
    }

    internal class MultiplicationToken : BinaryOperatorToken
    {
        public override string Pattern => @"mul ";
    }

    internal class DivisionToken : BinaryOperatorToken
    {
        public override string Pattern => "div ";
    }

    internal class ModulusToken : BinaryOperatorToken
    {
        public override string Pattern => "mod ";
    }

    internal class NumberToken : ValueToken
    {
        public override string Pattern => @"\d+";
    }

    internal class StringToken : ValueToken
    {
        public override string Pattern => "'[^']*'";
    }

    internal class VariableToken : ValueToken
    {
        public override string Pattern => "[a-zA-Z][a-zA-Z0-9_.]*";
    }

    internal class CommaToken : Token
    {
        public override string Pattern => ",";
    }

    internal class EpsilonToken : Token
    {
        public override string Pattern => "$";
    }
}
