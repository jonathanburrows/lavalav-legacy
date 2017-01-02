using System;

namespace lvl.Web.OData.Expressions
{
    internal abstract class BinaryOperatorExpression : IExpression
    {
        protected abstract string Operator { get; }
        private IExpression LeftArgument { get; }
        private IExpression RightArgument { get; set; }

        public BinaryOperatorExpression(IExpression leftArgument, IExpression rightArgument)
        {
            if (leftArgument == null)
            {
                throw new ArgumentNullException(nameof(leftArgument));
            }
            if (rightArgument == null)
            {
                throw new ArgumentNullException(nameof(rightArgument));
            }

            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }

        public string CsString() => $"({LeftArgument.CsString()}) {Operator} ({RightArgument.CsString()})";
    }
}
