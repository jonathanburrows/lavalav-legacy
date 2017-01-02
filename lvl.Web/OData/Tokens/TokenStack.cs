using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.OData.Tokens
{
    internal class TokenStack
    {
        private Queue<Token> Tokens { get; }

        public Token Lookahead => Tokens.FirstOrDefault() ?? new EpsilonToken();

        public TokenStack(IEnumerable<Token> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            Tokens = new Queue<Token>(tokens);
        }

        public void Next()
        {
            Tokens.Dequeue();
        }
    }
}
