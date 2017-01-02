using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Multiple classes were put into one file as they are all very thin.
    /// </summary>
    internal class AdditionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "+";

        public AdditionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class AndExpression : BinaryOperatorExpression
    {
        protected override string Operator => "&&";

        public AndExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class DivisionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "/";

        public DivisionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class EqualsExpression : BinaryOperatorExpression
    {
        protected override string Operator => "=";

        public EqualsExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class GreaterThanEqualExpression : BinaryOperatorExpression
    {
        protected override string Operator => ">=";

        public GreaterThanEqualExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class GreaterThanExpression : BinaryOperatorExpression
    {
        protected override string Operator => ">";

        public GreaterThanExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class LessThanEqualExpression : BinaryOperatorExpression
    {
        protected override string Operator => "<=";

        public LessThanEqualExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class LessThanExpression : BinaryOperatorExpression
    {
        protected override string Operator => "<";

        public LessThanExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class ModulusExpression : BinaryOperatorExpression
    {
        protected override string Operator => "%";

        public ModulusExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class MultiplicationExpression : BinaryOperatorExpression
    {
        protected override string Operator => "*";

        public MultiplicationExpression(IExpression leftExpression, IExpression rightExpression) : base(leftExpression, rightExpression) { }
    }

    internal class NotEqualsExpression : BinaryOperatorExpression
    {
        protected override string Operator => "!=";

        public NotEqualsExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class OrExpression : BinaryOperatorExpression
    {
        protected override string Operator => "||";

        public OrExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class SubtractionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "-";

        public SubtractionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class NumberExpression : ValueExpression
    {
        public int Value { get; set; }

        public NumberExpression(string value)
        {
            int _value;
            if (!int.TryParse(value, out _value))
            {
                throw new InvalidCastException($"Attempting to cast the value '{value}' as a number");
            }

            Value = _value;
        }

        public override string CsString() => Value.ToString();
    }

    internal class StringExpression : ValueExpression
    {
        private string Value { get; }

        public StringExpression(string value)
        {
            Value = value;
        }

        public override string CsString()
        {
            var valuePattern = new Regex("'(.*)'");
            var valueWithoutQuotes = valuePattern.Match(Value).Groups[1].Value;
            return $@"""{valueWithoutQuotes}""";
        }
    }

    internal class VariableExpression : ValueExpression
    {
        private string Name { get; set; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override string CsString() => Name;
    }

    internal class ConcatExpression : FunctionExpression
    {
        public ConcatExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"({Arg0.CsString()}) + ({Arg1.CsString()})";
    }

    internal class SubstringOfExpression : FunctionExpression
    {
        public SubstringOfExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg1.CsString()}.Contains({Arg2.CsString()})";
    }

    internal class EndsWithExpression : FunctionExpression
    {
        public EndsWithExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg1.CsString()}.EndsWith({Arg2.CsString()})";
    }

    internal class StartsWithExpression : FunctionExpression
    {
        public StartsWithExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg1.CsString()}.StartsWith({Arg2.CsString()})";
    }

    internal class LengthExpression : FunctionExpression
    {
        public LengthExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Length";
    }

    internal class IndexOfExpression : FunctionExpression
    {
        public IndexOfExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.IndexOf({Arg1.CsString()})";
    }

    internal class ReplaceExpression : FunctionExpression
    {
        public ReplaceExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Replace({Arg1.CsString()}, {Arg2.CsString()})";
    }

    internal class SubstringExpression : FunctionExpression
    {
        public SubstringExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Substring({Arg1.CsString()})";
    }

    internal class ToLowerExpression : FunctionExpression
    {
        public ToLowerExpression(IExpression arg0) : base(arg0) { }

        public override string CsString() => $"({Arg0.CsString()}).ToLower()";
    }

    internal class ToUpperExpression : FunctionExpression
    {
        public ToUpperExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.ToUpper()";
    }

    internal class TrimExpression : FunctionExpression
    {
        public TrimExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Trim()";
    }

    internal class DayExpression : FunctionExpression
    {
        public DayExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Day";
    }

    internal class HourExpression : FunctionExpression
    {
        public HourExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Hour";
    }

    internal class MinuteExpression : FunctionExpression
    {
        public MinuteExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Minute";
    }

    internal class SecondExpression : FunctionExpression
    {
        public SecondExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Second";
    }

    internal class YearExpression : FunctionExpression
    {
        public YearExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Year";
    }

    internal class RoundExpression : FunctionExpression
    {
        public RoundExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"Math.Round({Arg0.CsString()})";
    }

    internal class CeilingExpression : FunctionExpression
    {
        public CeilingExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"Math.Ceiling({Arg0.CsString()})";
    }

    internal class FloorExpression : FunctionExpression
    {
        public FloorExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"Math.Floor({Arg0.CsString()})";
    }

    public class SequenceExpression : IExpression
    {
        public Queue<IExpression> Expressions { get; }

        public SequenceExpression()
        {
            Expressions = new Queue<IExpression>();
        }

        public string CsString()
        {
            var expressionStrings = Expressions.Select(e => e.CsString());
            return $"{string.Join(",", expressionStrings)}";
        }
    }
}
