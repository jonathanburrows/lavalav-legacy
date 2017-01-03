namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Represents a value to be taken directly from a token.
    /// </summary>
    internal abstract class ValueExpression : IExpression
    {
        public abstract string CsString();
    }
}
