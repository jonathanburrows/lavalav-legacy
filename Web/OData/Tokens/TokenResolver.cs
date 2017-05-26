using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace lvl.Web.OData.Tokens
{
    /// <summary>
    ///     Stores construction information about a token, and the pattern it matches too.
    /// </summary>
    internal class TokenResolver
    {
        public Regex Pattern { get; }
        private ConstructorInfo Constructor { get; }

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

            Constructor = tokenType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null)
            {
                throw new ArgumentException($"{tokenType.Name} does not have a parameterless constructor.");
            }

            Pattern = new Regex($"^({pattern})");
        }

        /// <summary>
        ///     Creates a new token, and assigns the given value to it.
        /// </summary>
        /// <param name="sequence">The value to assign to the token.</param>
        /// <returns>The constructed token.</returns>
        public Token Resolve(string sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            var token = (Token)Constructor.Invoke(new object[0]);
            token.Value = sequence;
            return token;
        }
    }
}
