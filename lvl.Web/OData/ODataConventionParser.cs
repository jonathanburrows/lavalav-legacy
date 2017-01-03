using lvl.Web.OData.Expressions;
using lvl.Web.OData.Tokens;

namespace lvl.Web.OData
{
    /// <summary>
    /// Parses a set of odata tokens into a valid C# string
    /// </summary>
    public class ODataConventionParser : Parser
    {
        public ODataConventionParser()
        {
            RegisterLogical<AndToken, AndExpression>();
            RegisterLogical<OrToken, OrExpression>();

            RegisterUnary<NotToken, NotExpression>();
            RegisterUnary<PositiveSignToken, PositiveSignExpression>();
            RegisterUnary<NegativeSignToken, NegativeSignExpression>();

            RegisterBinaryOperator<AdditionToken, AdditionExpression>();
            RegisterBinaryOperator<SubtractionToken, SubtractionExpression>();
            RegisterBinaryOperator<MultiplicationToken, MultiplicationExpression>();
            RegisterBinaryOperator<DivisionToken, DivisionExpression>();
            RegisterBinaryOperator<ModulusToken, ModulusExpression>();

            RegisterComparison<EqualsToken, EqualsExpression>();
            RegisterComparison<NotEqualsToken, NotEqualsExpression>();
            RegisterComparison<GreaterThanToken, GreaterThanExpression>();
            RegisterComparison<GreaterThanEqualToken, GreaterThanEqualExpression>();
            RegisterComparison<LessThanToken, LessThanExpression>();
            RegisterComparison<LessThanEqualToken, LessThanEqualExpression>();

            RegisterFunction<ConcatToken, ConcatExpression>();
            RegisterFunction<SubstringOfToken, SubstringOfExpression>();
            RegisterFunction<SubstringToken, SubstringExpression>();
            RegisterFunction<EndsWithToken, EndsWithExpression>();
            RegisterFunction<StartsWithToken, StartsWithExpression>();
            RegisterFunction<LengthToken, LengthExpression>();
            RegisterFunction<IndexOfToken, IndexOfExpression>();
            RegisterFunction<ReplaceToken, ReplaceExpression>();
            RegisterFunction<ToLowerToken, ToLowerExpression>();
            RegisterFunction<ToUpperToken, ToUpperExpression>();
            RegisterFunction<TrimToken, TrimExpression>();
            RegisterFunction<DayToken, DayExpression>();
            RegisterFunction<HourToken, HourExpression>();
            RegisterFunction<MinuteToken, MinuteExpression>();
            RegisterFunction<SecondToken, SecondExpression>();
            RegisterFunction<YearToken, YearExpression>();
            RegisterFunction<RoundToken, RoundExpression>();
            RegisterFunction<CeilingToken, CeilingExpression>();
            RegisterFunction<FloorToken, FloorExpression>();

            RegisterValue<NumberToken, NumberExpression>();
            RegisterValue<StringToken, StringExpression>();
            RegisterValue<BooleanToken, BooleanExpression>();
            RegisterValue<NullToken, NullExpression>();
            RegisterValue<VariableToken, VariableExpression>();
        }
    }
}
