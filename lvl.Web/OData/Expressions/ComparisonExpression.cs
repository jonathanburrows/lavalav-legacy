using System;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Represents a comparison between two objects, with a boolean result.
    /// </summary>
    internal abstract class ComparisonExpression : IExpression
    {
        protected abstract string Operator { get; }
        protected IExpression LeftArgument { get; }
        protected IExpression RightArgument { get; set; }

        protected ComparisonExpression(IExpression leftArgument, IExpression rightArgument)
        {
            LeftArgument = leftArgument ?? throw new ArgumentNullException(nameof(leftArgument));
            RightArgument = rightArgument ?? throw new ArgumentNullException(nameof(rightArgument));
        }

        public virtual string CsString() => $"({LeftArgument.CsString()}) {Operator} ({RightArgument.CsString()})";
    }
}
