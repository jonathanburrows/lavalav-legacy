using System;

namespace lvl.Web.OData.Expressions
{
    internal abstract class BinaryOperatorExpression : IExpression
    {
        protected abstract string Operator { get; }
        protected IExpression LeftArgument { get; }
        protected IExpression RightArgument { get; set; }

        protected BinaryOperatorExpression(IExpression leftArgument, IExpression rightArgument)
        {
            LeftArgument = leftArgument ?? throw new ArgumentNullException(nameof(leftArgument));
            RightArgument = rightArgument ?? throw new ArgumentNullException(nameof(rightArgument));
        }

        public virtual string CsString() => $"({LeftArgument.CsString()}) {Operator} ({RightArgument.CsString()})";
    }
}
