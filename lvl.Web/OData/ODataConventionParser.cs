using lvl.Web.OData.Expressions;
using lvl.Web.OData.Tokens;
using System;
using System.Collections.Generic;

namespace lvl.Web.OData
{
    /// <summary>
    /// Parses a set of odata tokens into a valid C# string
    /// </summary>
    public class ODataConventionParser
    {
        public IExpression Parse(IEnumerable<Token> tokens)
        {
            var tokenStack = new TokenStack(tokens);
            var parsed = Expression(tokenStack);
            if (!(tokenStack.Lookahead is EpsilonToken))
            {
                throw new InvalidOperationException($"Unexpected token symbol '{tokenStack.Lookahead.GetType().Name}' with value '{tokenStack.Lookahead.Value}'");
            }
            return parsed;
        }

        private IExpression Expression(TokenStack tokens)
        {
            var argument = Function(tokens);
            return BinaryOperator(tokens, argument);
        }

        private IExpression BinaryOperator(TokenStack tokens, IExpression expression)
        {
            if (tokens.Lookahead is BinaryOperatorToken)
            {
                var operatorToken = tokens.Lookahead;
                tokens.Next();
                var rightArgument = Expression(tokens);

                if (operatorToken is AdditionToken)
                {
                    return new AdditionExpression(expression, rightArgument);
                }
                else if (operatorToken is SubtractionToken)
                {
                    return new SubtractionExpression(expression, rightArgument);
                }
                else if (operatorToken is MultiplicationToken)
                {
                    return new MultiplicationExpression(expression, rightArgument);
                }
                else if (operatorToken is DivisionToken)
                {
                    return new DivisionExpression(expression, rightArgument);
                }
                else if (operatorToken is ModulusToken)
                {
                    return new ModulusExpression(expression, rightArgument);
                }
                else if (operatorToken is EqualsToken)
                {
                    return new EqualsExpression(expression, rightArgument);
                }
                else if (operatorToken is NotEqualsToken)
                {
                    return new NotEqualsExpression(expression, rightArgument);
                }
                else if (operatorToken is GreaterThanToken)
                {
                    return new GreaterThanExpression(expression, rightArgument);
                }
                else if (operatorToken is GreaterThanEqualToken)
                {
                    return new GreaterThanEqualExpression(expression, rightArgument);
                }
                else if (operatorToken is LessThanToken)
                {
                    return new LessThanExpression(expression, rightArgument);
                }
                else if (operatorToken is LessThanEqualToken)
                {
                    return new LessThanEqualExpression(expression, rightArgument);
                }
                else if (operatorToken is AndToken)
                {
                    return new AndExpression(expression, rightArgument);
                }
                else if (operatorToken is OrToken)
                {
                    return new OrExpression(expression, rightArgument);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported binary token '{operatorToken.GetType().Name}' with value '{operatorToken.Value}'");
                }
            }
            else if (tokens.Lookahead is CommaToken)
            {
                var sequenceExpression = expression as SequenceExpression;
                if (sequenceExpression == null)
                {
                    sequenceExpression = new SequenceExpression();
                    sequenceExpression.Expressions.Enqueue(expression);
                }
                tokens.Next();
                var next = Expression(tokens);
                sequenceExpression.Expressions.Enqueue(next);
                return sequenceExpression;
            }
            else
            {
                return expression;
            }
        }

        private IExpression Function(TokenStack tokens)
        {
            if (tokens.Lookahead is FunctionToken)
            {
                var functionToken = tokens.Lookahead;
                tokens.Next();

                var arguments = Function(tokens);

                if (functionToken is ConcatToken)
                {
                    return new ConcatExpression(arguments);
                }
                else if (functionToken is SubstringOfToken)
                {
                    return new SubstringOfExpression(arguments);
                }
                else if (functionToken is SubstringToken)
                {
                    return new SubstringExpression(arguments);
                }
                else if (functionToken is EndsWithToken)
                {
                    return new EndsWithExpression(arguments);
                }
                else if (functionToken is StartsWithToken)
                {
                    return new StartsWithExpression(arguments);
                }
                else if (functionToken is LengthToken)
                {
                    return new LengthExpression(arguments);
                }
                else if (functionToken is IndexOfToken)
                {
                    return new IndexOfExpression(arguments);
                }
                else if (functionToken is ReplaceToken)
                {
                    return new ReplaceExpression(arguments);
                }
                else if (functionToken is SubstringToken)
                {
                    return new SubstringExpression(arguments);
                }
                else if (functionToken is ToLowerToken)
                {
                    return new ToLowerExpression(arguments);
                }
                else if (functionToken is ToUpperToken)
                {
                    return new ToUpperExpression(arguments);
                }
                else if (functionToken is TrimToken)
                {
                    return new TrimExpression(arguments);
                }
                else if (functionToken is DayToken)
                {
                    return new TrimExpression(arguments);
                }
                else if (functionToken is HourToken)
                {
                    return new HourExpression(arguments);
                }
                else if (functionToken is MinuteToken)
                {
                    return new MinuteExpression(arguments);
                }
                else if (functionToken is SecondToken)
                {
                    return new SecondExpression(arguments);
                }
                else if (functionToken is YearToken)
                {
                    return new YearExpression(arguments);
                }
                else if (functionToken is RoundToken)
                {
                    return new RoundExpression(arguments);
                }
                else if (functionToken is CeilngToken)
                {
                    return new CeilingExpression(arguments);
                }
                else if (functionToken is FloorToken)
                {
                    return new FloorExpression(arguments);
                }
                else
                {
                    throw new InvalidOperationException($"Could not parse the function {functionToken.Value}");
                }
            }
            else if (tokens.Lookahead is OpenBracketToken)
            {
                tokens.Next();
                var expression = Expression(tokens);

                if (!(tokens.Lookahead is CloseBracketToken))
                {
                    throw new InvalidOperationException($"Closing brackets expected, instead got a {tokens.Lookahead.GetType().Name} of value {tokens.Lookahead.Value}");
                }

                tokens.Next();
                return expression;
            }

            return Value(tokens);
        }

        private IExpression Value(TokenStack tokens)
        {
            if (tokens.Lookahead is NumberToken)
            {
                var numberExpression = new NumberExpression(tokens.Lookahead.Value);
                tokens.Next();
                return numberExpression;
            }
            else if (tokens.Lookahead is StringToken)
            {
                var stringExpression = new StringExpression(tokens.Lookahead.Value);
                tokens.Next();
                return stringExpression;
            }
            else if (tokens.Lookahead is VariableToken)
            {
                var variableExpression = new VariableExpression(tokens.Lookahead.Value);
                tokens.Next();
                return variableExpression;
            }
            else if (tokens.Lookahead is EpsilonToken)
            {
                throw new InvalidOperationException("Unexpected end of input.");
            }
            else
            {
                throw new InvalidOperationException($"Unexpected {tokens.Lookahead.GetType().Name} symbol of value {tokens.Lookahead.Value} found");
            }
        }
    }
}
