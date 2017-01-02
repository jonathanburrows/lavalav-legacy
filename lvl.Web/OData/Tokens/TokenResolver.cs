using System;
using System.Text.RegularExpressions;

namespace lvl.Web.OData.Tokens
{
    internal class TokenResolver
    {
        public Regex Pattern { get; }
        private Type TokenType { get; }

        public TokenResolver(Type tokenType, string pattern)
        {
            if (tokenType == null)
            {
                throw new ArgumentNullException(nameof(tokenType));
            }
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            TokenType = tokenType;
            Pattern = new Regex($"^({pattern})");
        }

        public Token Construct(string sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            var token = (Token)Activator.CreateInstance(TokenType);
            token.Value = sequence;
            return token;
        }
    }
}
