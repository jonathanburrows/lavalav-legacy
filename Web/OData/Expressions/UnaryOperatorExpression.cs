using System;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Represents an operator to be applied to a single operand.
    /// </summary>
    internal abstract class UnaryOperatorExpression : IExpression
    {
        protected IExpression Argument { get; }

        protected UnaryOperatorExpression(IExpression argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract string CsString();
    }
}
