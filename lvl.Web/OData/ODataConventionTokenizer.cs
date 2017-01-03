using lvl.Web.OData.Tokens;

namespace lvl.Web.OData
{
    /// <summary>
    /// Converts a query string into a set of tokens for an abstract syntax tree.
    /// </summary>
     public class ODataConventionTokenizer : Tokenizer
    {
        public ODataConventionTokenizer()
        {
            Register<AdditionToken>();
            Register<SubtractionToken>();
            Register<MultiplicationToken>();
            Register<DivisionToken>();
            Register<ModulusToken>();

            Register<PositiveSignToken>();
            Register<NegativeSignToken>();
            Register<NotToken>();

            Register<CommaToken>();
            Register<SubstringOfToken>();
            Register<EndsWithToken>();
            Register<StartsWithToken>();
            Register<LengthToken>();
            Register<IndexOfToken>();
            Register<ReplaceToken>();
            Register<SubstringToken>();
            Register<ToLowerToken>();
            Register<ToUpperToken>();
            Register<TrimToken>();
            Register<ConcatToken>();
            Register<DayToken>();
            Register<HourToken>();
            Register<MinuteToken>();
            Register<MonthToken>();
            Register<SecondToken>();
            Register<YearToken>();
            Register<RoundToken>();
            Register<FloorToken>();
            Register<CeilingToken>();

            Register<OpenBracketToken>();
            Register<CloseBracketToken>();

            Register<NotToken>();
            Register<EqualsToken>();
            Register<NotEqualsToken>();
            Register<GreaterThanToken>();
            Register<GreaterThanEqualToken>();
            Register<LessThanToken>();
            Register<LessThanEqualToken>();
            Register<AndToken>();
            Register<OrToken>();

            Register<NumberToken>();
            Register<StringToken>();
            Register<NullToken>();
            Register<BooleanToken>();
            Register<VariableToken>();
        }
    }
}
