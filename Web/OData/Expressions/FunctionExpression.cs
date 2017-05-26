namespace lvl.Web.OData.Expressions
{
    /// <summary>
    ///     Represents a function, which will consume a list of arguments.
    /// </summary>
    internal abstract class FunctionExpression : IExpression
    {
        protected IExpression Arg0 { get; }
        protected IExpression Arg1 { get; }
        protected IExpression Arg2 { get; }

        protected FunctionExpression(IExpression argument)
        {
            var sequence = argument as SequenceExpression;
            if (sequence != null)
            {
                var arguments = sequence.Expressions.ToArray();
                if (arguments.Length > 0)
                {
                    Arg0 = arguments[0];
                }
                if (arguments.Length > 1)
                {
                    Arg1 = arguments[1];
                }
                if (arguments.Length > 2)
                {
                    Arg2 = arguments[2];
                }
            }
            else
            {
                Arg0 = argument;
            }

        }

        public abstract string CsString();
    }
}
