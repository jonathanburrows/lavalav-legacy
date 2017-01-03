using lvl.Web.OData.Tokens;
using System;
using System.Collections.Generic;

namespace lvl.Web.OData.Expressions
{
    /// <summary>
    /// Parses a set of odata tokens into a valid C# string
    /// </summary>
    public class Parser
    {
        private delegate IExpression BinaryArgumentResolver(IExpression leftArgument, IExpression rightArgument);
        private delegate IExpression UnaryArgumentResolver(IExpression arguments);
        private IDictionary<Type, BinaryArgumentResolver> LogicalExpressionResolvers { get; }
        private IDictionary<Type, BinaryArgumentResolver> BinaryOperatorExpressionResolvers { get; }
        private IDictionary<Type, BinaryArgumentResolver> ComparisonExpressionResolvers { get; }
        private IDictionary<Type, UnaryArgumentResolver> FunctionExpressionResolvers { get; }
        private IDictionary<Type, Func<string, IExpression>> ValueExpressionResolvers { get; }
        private IDictionary<Type, UnaryArgumentResolver> UnaryExpressionResolvers { get; }

        public Parser()
        {
            LogicalExpressionResolvers = new Dictionary<Type, BinaryArgumentResolver>();
            BinaryOperatorExpressionResolvers = new Dictionary<Type, BinaryArgumentResolver>();
            ComparisonExpressionResolvers = new Dictionary<Type, BinaryArgumentResolver>();
            FunctionExpressionResolvers = new Dictionary<Type, UnaryArgumentResolver>();
            ValueExpressionResolvers = new Dictionary<Type, Func<string, IExpression>>();
            UnaryExpressionResolvers = new Dictionary<Type, UnaryArgumentResolver>();
        }

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
            return ExpressionOp(tokens, argument);
        }

        private IExpression ExpressionOp(TokenStack tokens, IExpression expression)
        {
            if (tokens.Lookahead is EpsilonToken)
            {
                return expression;
            }
            if (tokens.Lookahead is CloseBracketToken)
            {
                return expression;
            }

            var logical = Logical(tokens, expression);
            var comparison = Comparison(tokens, logical);
            var binaryOperator = BinaryOperator(tokens, comparison);
            var sequence = Sequence(tokens, binaryOperator);

            if (expression == sequence)
            {
                throw new InvalidOperationException($"Unexpected token '{tokens.Lookahead.Value}'  of type {tokens.Lookahead.GetType().Name}");
            }
            return ExpressionOp(tokens, sequence);
        }

        private IExpression NonBooleanExpression(TokenStack tokens)
        {
            var argument = Function(tokens);
            var binaryOperator = BinaryOperator(tokens, argument);
            var sequence = Sequence(tokens, binaryOperator);
            return sequence;
        }

        private IExpression Logical(TokenStack tokens, IExpression expression)
        {
            if (!(tokens.Lookahead is LogicalToken))
            {
                return expression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!LogicalExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported logical token '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }

            var resolver = LogicalExpressionResolvers[tokenType];
            tokens.Next();
            var rightArgument = Expression(tokens);
            return resolver(expression, rightArgument);
        }

        private IExpression Comparison(TokenStack tokens, IExpression expression)
        {
            if (!(tokens.Lookahead is ComparisonToken))
            {
                return expression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!ComparisonExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported comparison operator '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }

            var resolver = ComparisonExpressionResolvers[tokenType];
            tokens.Next();
            var rightArgument = Expression(tokens);
            return resolver(expression, rightArgument);
        }

        private IExpression BinaryOperator(TokenStack tokens, IExpression expression)
        {
            if (!(tokens.Lookahead is BinaryOperatorToken))
            {
                return expression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!BinaryOperatorExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported binary expression '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }
            var resolver = BinaryOperatorExpressionResolvers[tokenType];
            tokens.Next();
            var rightArgument = NonBooleanExpression(tokens);
            return resolver(expression, rightArgument);
        }

        private IExpression Sequence(TokenStack tokens, IExpression expression)
        {
            if (!(tokens.Lookahead is CommaToken))
            {
                return expression;
            }

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

        private IExpression Function(TokenStack tokens)
        {
            if (tokens.Lookahead is FunctionToken)
            {
                var tokenType = tokens.Lookahead.GetType();
                if (!FunctionExpressionResolvers.ContainsKey(tokenType))
                {
                    throw new InvalidOperationException($"Unsupported function token '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
                }
                var resolver = FunctionExpressionResolvers[tokenType];
                tokens.Next();
                var arguments = Expression(tokens);
                return resolver(arguments);
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
            else
            {
                return Value(tokens);
            }
        }

        private IExpression Value(TokenStack tokens)
        {
            if (tokens.Lookahead is EpsilonToken)
            {
                throw new InvalidOperationException("Unexpected end of input.");
            }
            if (tokens.Lookahead is UnaryOperatorToken)
            {
                var tokenType = tokens.Lookahead.GetType();
                if (!UnaryExpressionResolvers.ContainsKey(tokenType))
                {
                    throw new InvalidOperationException($"Unsupported unary type '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
                }
                var resolver = UnaryExpressionResolvers[tokenType];
                tokens.Next();
                var argument = Expression(tokens);
                return resolver(argument);
            }
            if (tokens.Lookahead is ValueToken)
            {
                var tokenType = tokens.Lookahead.GetType();
                if (!ValueExpressionResolvers.ContainsKey(tokenType))
                {
                    throw new InvalidOperationException($"Unsupported value type '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
                }
                var value = tokens.Lookahead.Value;
                var resolver = ValueExpressionResolvers[tokenType];
                var valueExpression = resolver(value);
                tokens.Next();
                return valueExpression;
            }
            else
            {
                throw new InvalidOperationException($"Unexpected {tokens.Lookahead.GetType().Name} symbol of value {tokens.Lookahead.Value} found");
            }
        }

        internal Parser RegisterLogical<TToken, TExpression>() where TToken : LogicalToken where TExpression : LogicalExpression
        {
            LogicalExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
            {
                var resolverType = typeof(TExpression);
                var resolver = Activator.CreateInstance(resolverType, leftArgument, rightArgument);
                return (IExpression)resolver;
            };
            return this;
        }

        internal Parser RegisterBinaryOperator<TToken, TExpression>() where TToken : BinaryOperatorToken where TExpression : BinaryOperatorExpression
        {
            BinaryOperatorExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
            {
                var resolverType = typeof(TExpression);
                var resolver = Activator.CreateInstance(resolverType, leftArgument, rightArgument);
                return (IExpression)resolver;
            };
            return this;
        }

        internal Parser RegisterComparison<TToken, TExpression>() where TToken : ComparisonToken where TExpression : ComparisonExpression
        {
            ComparisonExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
            {
                var resolverType = typeof(TExpression);
                var resolver = Activator.CreateInstance(resolverType, leftArgument, rightArgument);
                return (IExpression)resolver;
            };
            return this;
        }

        internal Parser RegisterFunction<TToken, TExpression>() where TToken : FunctionToken where TExpression : FunctionExpression
        {
            FunctionExpressionResolvers[typeof(TToken)] = (arguments) =>
            {
                var resolverType = typeof(TExpression);
                var resolver = Activator.CreateInstance(resolverType, arguments);
                return (IExpression)resolver;
            };
            return this;
        }

        internal Parser RegisterValue<TToken, TExpression>() where TToken : ValueToken where TExpression : ValueExpression
        {
            ValueExpressionResolvers[typeof(TToken)] = (value) =>
            {
                var resolverType = typeof(TExpression);
                var resolved = Activator.CreateInstance(resolverType, value);
                return (IExpression)resolved;
            };
            return this;
        }

        internal Parser RegisterUnary<TToken, TExpression>() where TToken : UnaryOperatorToken where TExpression : UnaryOperatorExpression
        {
            UnaryExpressionResolvers[typeof(TToken)] = (value) =>
            {
                var resolverType = typeof(TExpression);
                var resolved = Activator.CreateInstance(resolverType, value);
                return (IExpression)resolved;
            };
            return this;
        }
    }
}
