using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.OData.Tokens
{
    /// <summary>Converts a string input into a set of tokens.</summary>
    public class Tokenizer
    {
        private List<TokenResolver> TokenResolvers { get; } = new List<TokenResolver>();

        public Tokenizer Register<TToken>() where TToken : Token, new()
        {
            var instance = new TToken();
            var pattern = instance.Pattern;
            var type = typeof(TToken);
            var tokenResolver = new TokenResolver(type, pattern);
            TokenResolvers.Add(tokenResolver);
            return this;
        }

        /// <summary>
        /// Converts an input string into a set of tokens.
        /// </summary>
        /// <param name="tokenizing">The input to be tokenized.</param>
        /// <returns>The set of tokens produced.</returns>
        /// <exception cref="InvalidOperationException">A character was encountered which could not be tokenized.</exception>
        public IEnumerable<Token> Tokenize(string tokenizing)
        {
            if (tokenizing == string.Empty)
            {
                return Enumerable.Empty<Token>();
            }

            foreach (var tokenResolver in TokenResolvers)
            {
                var pattern = tokenResolver.Pattern;
                if (pattern.IsMatch(tokenizing))
                {
                    var value = pattern.Match(tokenizing).Value.Trim();
                    var token = tokenResolver.Resolve(value);
                    var remainingString = pattern.Replace(tokenizing, string.Empty, 1).Trim();
                    var remainingTokens = Tokenize(remainingString);

                    return new[] { token }.Union(remainingTokens);
                }
            }

            throw new InvalidOperationException($"Unexpected character in odata input '{tokenizing}'");
        }
    }
}
