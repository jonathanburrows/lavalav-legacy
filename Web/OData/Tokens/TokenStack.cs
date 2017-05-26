using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.OData.Tokens
{
    /// <summary>
    ///     Represents a set of tokens which must be reduced by the parser.
    /// </summary>
    internal class TokenStack
    {
        private Queue<Token> Tokens { get; }

        /// <summary>
        ///     Allows the parser to peek at the next token.
        /// </summary>
        public Token Lookahead => Tokens.FirstOrDefault() ?? new EpsilonToken();

        public TokenStack(IEnumerable<Token> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            Tokens = new Queue<Token>(tokens);
        }

        /// <summary>
        ///     Signifies the parser has consumed the first token in the stack.
        /// </summary>
        public void Next()
        {
            Tokens.Dequeue();
        }
    }
}
