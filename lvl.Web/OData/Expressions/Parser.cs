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

        /// <summary>
        /// Parses a set of tokens into an abstract syntax tree.
        /// </summary>
        /// <param name="tokens">The tokens which will be used to construct the syntax tree.</param>
        /// <returns>The root node of the syntax tree.</returns>
        /// <exception cref="InvalidOperationException">The tokens could not be parsed into a valid tree.</exception>
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

        /// <summary>Reduces a stack of tokens into a single expression.</summary>
        private IExpression Expression(TokenStack tokens)
        {
            var argument = Function(tokens);
            return ExpressionOp(tokens, argument);
        }

        /// <summary>Recursively reduces a token stack into an expression.</summary>
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

        /// <summary>Allows for arithmetic expressions to be parsed, without evaluating logical operators.</summary>
        private IExpression NonBooleanExpression(TokenStack tokens)
        {
            var argument = Function(tokens);
            var binaryOperator = BinaryOperator(tokens, argument);
            var sequence = Sequence(tokens, binaryOperator);
            return sequence;
        }

        /// <summary>If the next token is a logical operator, constructs a logical expression from the previous and next expression.</summary>
        private IExpression Logical(TokenStack tokens, IExpression previousExpression)
        {
            if (!(tokens.Lookahead is LogicalToken))
            {
                return previousExpression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!LogicalExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported logical token '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }

            var resolver = LogicalExpressionResolvers[tokenType];
            tokens.Next();
            var nextExpression = Expression(tokens);
            return resolver(previousExpression, nextExpression);
        }

        /// <summary>If the next token is a comparison operator, constructs a comparison expression from the previous and next expression.</summary>
        private IExpression Comparison(TokenStack tokens, IExpression previousExpression)
        {
            if (!(tokens.Lookahead is ComparisonToken))
            {
                return previousExpression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!ComparisonExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported comparison operator '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }

            var resolver = ComparisonExpressionResolvers[tokenType];
            tokens.Next();
            var nextExpression = Expression(tokens);
            return resolver(previousExpression, nextExpression);
        }

        /// <summary>If the next token is a binary operator, constructs a binary expression with the previous and next expressions.</summary>
        private IExpression BinaryOperator(TokenStack tokens, IExpression previousExpression)
        {
            if (!(tokens.Lookahead is BinaryOperatorToken))
            {
                return previousExpression;
            }

            var tokenType = tokens.Lookahead.GetType();
            if (!BinaryOperatorExpressionResolvers.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"Unsupported binary expression '{tokens.Lookahead.GetType().Name}' with value '{tokens.Lookahead.Value}'");
            }
            var resolver = BinaryOperatorExpressionResolvers[tokenType];
            tokens.Next();
            var rightArgument = NonBooleanExpression(tokens);
            return resolver(previousExpression, rightArgument);
        }

        /// <summary>Constructs a list of values from tokens seperated by commas</summary>
        private IExpression Sequence(TokenStack tokens, IExpression expression)
        {
            if (!(tokens.Lookahead is CommaToken))
            {
                return expression;
            }

            tokens.Next();
            var next = Expression(tokens);

            if (next is SequenceExpression)
            {
                //The next expression is a sequence, to make sure it is flat, the previous expression is put at head of sequence.
                var sequenceExpression = (next as SequenceExpression);
                sequenceExpression.Expressions.Push(expression);
                return sequenceExpression;
            }
            else
            {
                //Construct a new seqence with two values, the previous and next expression.
                var sequenceExpression = new SequenceExpression();
                sequenceExpression.Expressions.Push(next);
                sequenceExpression.Expressions.Push(expression);
                return sequenceExpression;
            }
        }

        /// <summary>
        /// If the next token is a function, then a function expression is created.
        /// Whatever expressions are in the following brackets are passed as an argument.
        /// </summary>
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
                var arguments = Function(tokens);
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

        /// <summary>If the next token is a value token, then a value is parsed with that tokens value.</summary>
        /// <remarks>Also detects unary tokens. If a unary token is detected, it is interpreted as a value.</remarks>
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

        /// <summary>
        /// Signifies that a logical token should be parsed into a given logical expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A logical expression will be applied to its left and right hand expressions.</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking 2 IExpression parameters.</exception>
        internal Parser RegisterLogical<TToken, TExpression>() where TToken : LogicalToken where TExpression : LogicalExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(IExpression), typeof(IExpression) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting 2 {nameof(IExpression)}s");
            }

            LogicalExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
                (TExpression)constructor.Invoke(new[] { leftArgument, rightArgument });

            return this;
        }

        /// <summary>
        /// Signifies that a binary token should be parsed into a given binary expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A binary expression will be applied to its left and right hand expressions.</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking 2 IExpression parameters.</exception>
        internal Parser RegisterBinaryOperator<TToken, TExpression>() where TToken : BinaryOperatorToken where TExpression : BinaryOperatorExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(IExpression), typeof(IExpression) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting 2 {nameof(IExpression)}s");
            }

            BinaryOperatorExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
                (TExpression)constructor.Invoke(new[] { leftArgument, rightArgument });

            return this;
        }

        /// <summary>
        /// Signifies that a comparison token should be parsed into a given comparison expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A binary expression will be applied to its left and right hand expressions.</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking 2 IExpression parameters.</exception>
        internal Parser RegisterComparison<TToken, TExpression>() where TToken : ComparisonToken where TExpression : ComparisonExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(IExpression), typeof(IExpression) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting 2 {nameof(IExpression)}s");
            }

            ComparisonExpressionResolvers[typeof(TToken)] = (leftArgument, rightArgument) =>
                (TExpression)constructor.Invoke(new[] { leftArgument, rightArgument });

            return this;
        }

        /// <summary>
        /// Signifies that a function token should be parsed into a function expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A binary expression will be given a single argument</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking an IExpression parameter.</exception>
        internal Parser RegisterFunction<TToken, TExpression>() where TToken : FunctionToken where TExpression : FunctionExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(IExpression) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting an {nameof(IExpression)}");
            }

            FunctionExpressionResolvers[typeof(TToken)] = (arguments) =>
                (TExpression)constructor.Invoke(new[] { arguments });

            return this;
        }

        /// <summary>
        /// Signifies that a value token should be parsed into a value expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A value expression will be constructed from a string value.</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking a string parameter.</exception>
        internal Parser RegisterValue<TToken, TExpression>() where TToken : ValueToken where TExpression : ValueExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(string) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting a {nameof(String)}");
            }

            ValueExpressionResolvers[typeof(TToken)] = (value) =>
                (TExpression)constructor.Invoke(new[] { value });

            return this;
        }

        /// <summary>
        /// Signifies that a unary token should be parsed into a unary expression.
        /// </summary>
        /// <typeparam name="TToken">The type of token to be parsed.</typeparam>
        /// <typeparam name="TExpression">The type of expression to be constructed when the token is encountered.</typeparam>
        /// <returns>The original parser.</returns>
        /// <remarks>A unary expression will be applied to the following expression.</remarks>
        /// <exception cref="ArgumentException">The given expression type does not have a constructor taking an IExpression parameter.</exception>
        internal Parser RegisterUnary<TToken, TExpression>() where TToken : UnaryOperatorToken where TExpression : UnaryOperatorExpression
        {
            var constructor = typeof(TExpression).GetConstructor(new[] { typeof(IExpression) });
            if (constructor == null)
            {
                throw new ArgumentException($"{typeof(TExpression).Name} does not have a constructor accepting an {nameof(IExpression)}");
            }

            UnaryExpressionResolvers[typeof(TToken)] = (arguments) =>
                (TExpression)constructor.Invoke(new[] { arguments });

            return this;
        }
    }
}
