namespace lvl.Web.OData.Expressions
{
    /// <summary>
    ///     Represents an abstract syntax tree, which can be evaluated. Used to convert OData into C# code.
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        ///     Gets a legal C# equivilant string.
        /// </summary>
        string CsString();
    }
}
