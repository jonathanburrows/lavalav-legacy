using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Multiple classes were put into one file as they are all very thin.
    /// </summary>
    internal class AndExpression : LogicalExpression
    {
        protected override string Operator => "&&";

        public AndExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class OrExpression : LogicalExpression
    {
        protected override string Operator => "||";

        public OrExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class EqualsExpression : ComparisonExpression
    {
        protected override string Operator => "=";

        public EqualsExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class NotEqualsExpression : ComparisonExpression
    {
        protected override string Operator => "!=";

        public NotEqualsExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }

        public override string CsString()
        {
            var basicCompliment = base.CsString();
            if (LeftArgument is NullExpression || RightArgument is NullExpression)
            {
                return basicCompliment;
            }
            else
            {
                return $"({basicCompliment}) || ({LeftArgument.CsString()} = null) || ({RightArgument.CsString()} = null)";
            }
        }
    }

    internal class GreaterThanEqualExpression : ComparisonExpression
    {
        protected override string Operator => ">=";

        public GreaterThanEqualExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class GreaterThanExpression : ComparisonExpression
    {
        protected override string Operator => ">";

        public GreaterThanExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class LessThanEqualExpression : ComparisonExpression
    {
        protected override string Operator => "<=";

        public LessThanEqualExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class LessThanExpression : ComparisonExpression
    {
        protected override string Operator => "<";

        public LessThanExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }
    internal class AdditionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "+";

        public AdditionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class SubtractionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "-";

        public SubtractionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
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

    internal class DivisionExpression : BinaryOperatorExpression
    {
        protected override string Operator => "/";

        public DivisionExpression(IExpression leftArgument, IExpression rightArgument) : base(leftArgument, rightArgument) { }
    }

    internal class PositiveSignExpression : UnaryOperatorExpression
    {
        public PositiveSignExpression(IExpression argument) : base(argument) { }

        public override string CsString() => Argument.CsString();
    }

    internal class NegativeSignExpression : UnaryOperatorExpression
    {
        public NegativeSignExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"-1 * ({Argument.CsString()})";
    }

    internal class NotExpression : UnaryOperatorExpression
    {
        public NotExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"!({Argument.CsString()})";
    }

    internal class NumberExpression : ValueExpression
    {
        public int Value { get; set; }

        public NumberExpression(string value)
        {
            int val;
            if (!int.TryParse(value, out val))
            {
                throw new InvalidCastException($"Attempting to cast the value '{value}' as a number");
            }

            Value = val;
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
            var valueWithoutEscapes = valueWithoutQuotes.Replace("\\'", "'");
            return $@"""{valueWithoutEscapes}""";
        }
    }

    internal class NullExpression : ValueExpression
    {
        // ReSharper disable once UnusedParameter.Local Needed for reflection.
        public NullExpression(string value) { }

        public override string CsString() => "null";
    }

    internal class BooleanExpression : ValueExpression
    {
        private bool Value { get; }

        public BooleanExpression(string value)
        {
            bool val;
            if (!bool.TryParse(value, out val))
            {
                throw new ArgumentException($"'{value}' is not a value boolean value");
            }
            Value = val;
        }

        public override string CsString() => Value.ToString();
    }

    internal class VariableExpression : ValueExpression
    {
        private string Name { get; }

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

        public override string CsString() => $"{Arg0.CsString()}.Contains({Arg1.CsString()})";
    }

    internal class EndsWithExpression : FunctionExpression
    {
        public EndsWithExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.EndsWith({Arg1.CsString()})";
    }

    internal class StartsWithExpression : FunctionExpression
    {
        public StartsWithExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.StartsWith({Arg1.CsString()})";
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

        public override string CsString()
        {
            if (Arg2 != null)
            {
                return $"{Arg0.CsString()}.Substring({Arg1.CsString()}, {Arg2.CsString()})";
            }
            else
            {
                return $"{Arg0.CsString()}.Substring({Arg1.CsString()})";
            }
        }
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

        public override string CsString() => $"{Arg0.CsString()}.Value.Day";
    }

    internal class HourExpression : FunctionExpression
    {
        public HourExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Value.Hour";
    }

    internal class MinuteExpression : FunctionExpression
    {
        public MinuteExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Value.Minute";
    }

    internal class SecondExpression : FunctionExpression
    {
        public SecondExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Value.Second";
    }

    internal class YearExpression : FunctionExpression
    {
        public YearExpression(IExpression argument) : base(argument) { }

        public override string CsString() => $"{Arg0.CsString()}.Value.Year";
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
        public Stack<IExpression> Expressions { get; }

        public SequenceExpression()
        {
            Expressions = new Stack<IExpression>();
        }

        public string CsString()
        {
            var expressionStrings = Expressions.Select(e => e.CsString());
            return $"{string.Join(",", expressionStrings)}";
        }
    }
}
