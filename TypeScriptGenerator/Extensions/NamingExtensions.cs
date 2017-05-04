using System;
using System.Text;

namespace lvl.TypescriptGenerator.Extensions
{
    /// <summary>
    /// Will provide conversions from one naming convention to another.
    /// </summary>
    public static class NamingExtensions
    {
        /// <summary>
        /// Will make the first letter lowercase.
        /// </summary>
        /// <param name="converting">The name to be converted to pascal.</param>
        /// <returns>The pascal version of the given name.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converting"/> is null.</exception>
        internal static string ToPascal(this string converting)
        {
            if (converting == null)
            {
                throw new ArgumentNullException(nameof(converting));
            }

            if (converting == string.Empty)
            {
                return converting;
            }

            var pascalBuilder = new StringBuilder(converting);
            pascalBuilder[0] = char.ToLower(pascalBuilder[0]);
            return pascalBuilder.ToString();
        }

        /// <summary>
        /// Will seperate words by dashes.
        /// </summary>
        /// <param name="converting">The camel case name to be converted.</param>
        /// <returns>A name with each word seperated by a dash, and lowercased.</returns>
        /// <remarks>If two uppercase letters are side by side, it will not insert a dash between</remarks>
        internal static string ToDashed(this string converting)
        {
            var dashedName = new StringBuilder();

            for (var i = 0; i < converting.Length; i++)
            {
                if (i >= 1 && char.IsUpper(converting[i]) && char.IsLower(converting[i - 1]))
                {
                    dashedName.Append('-');
                }

                dashedName.Append(char.ToLower(converting[i]));
            }

            return dashedName.ToString();
        }
    }
}
