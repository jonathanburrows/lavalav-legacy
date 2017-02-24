using System;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Represents boolean logic between two expressions.
    /// </summary>
    internal abstract class LogicalExpression : IExpression
    {
        protected abstract string Operator { get; }
        protected IExpression LeftArgument { get; }
        protected IExpression RightArgument { get; set; }

        protected LogicalExpression(IExpression leftArgument, IExpression rightArgument)
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

        public virtual string CsString() => $"({LeftArgument.CsString()}) {Operator} ({RightArgument.CsString()})";
    }
}
