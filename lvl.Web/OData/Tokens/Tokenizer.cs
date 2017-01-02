using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.OData.Tokens
{
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
                    var token = tokenResolver.Construct(value);
                    var remainingString = pattern.Replace(tokenizing, string.Empty, 1).Trim();
                    var remainingTokens = Tokenize(remainingString);

                    return new[] { token }.Union(remainingTokens);
                }
            }

            throw new InvalidOperationException($"Unexpected character in odata input '{tokenizing}'");
        }
    }
}
