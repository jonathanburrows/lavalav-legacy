using System;

namespace lvl.Web.OData.Expressions
{
    internal abstract class UnaryOperatorExpression : IExpression
    {
        protected IExpression Argument { get; }

        public UnaryOperatorExpression(IExpression argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }
            Argument = argument;
        }

        public abstract string CsString();
    }
}
