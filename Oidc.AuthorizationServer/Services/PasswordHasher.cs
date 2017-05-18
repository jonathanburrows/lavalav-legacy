using System;
using System.Security.Cryptography;
using System.Text;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Will hash passwords and generate salts for passwords.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        ///     The length of the randomly generated salt.
        /// </summary>
        private const int SaltLength = 16;

        /// <summary>
        ///     Will salt, pepper, and hash a password.
        /// </summary>
        /// <param name="password">The password which will be salted, peppered, hashed.</param>
        /// <param name="salt">The salt to apply to the password.</param>
        /// <returns>The salted, peppered, and hashed password.</returns>
        public string Hash(string password, string salt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if(salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            var pepperedPassword = ApplyPepper(password, salt);
            return HashPepperedPassword(pepperedPassword);
        }

        /// <summary>
        ///     Generates a random string value used for cryptography.
        /// </summary>
        /// <returns>The randomly generated salt.</returns>
        public string GetSalt()
        {
            var salt = new byte[SaltLength * 2];
            using (var saltGenerator = RandomNumberGenerator.Create())
            {
                saltGenerator.GetBytes(salt);
                return BitConverter.ToString(salt).Replace("-", string.Empty);
            }
        }

        /// <summary>
        ///     Checks if a given password matches the stored password.
        /// </summary>
        /// <param name="hashedPassword">The password which is stored.</param>
        /// <param name="providedPassword">The password which will be checked against the one stored.</param>
        /// <param name="salt">The value which modifies the hashing.</param>
        /// <returns>True if the given password matches, false otherwise.</returns>
        public bool Verify(string hashedPassword, string providedPassword, string salt)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }
            if(salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            var givenPasswordPeppered = ApplyPepper(providedPassword, salt);
            var givenPasswordHashed = HashPepperedPassword(givenPasswordPeppered);
            return hashedPassword == givenPasswordHashed;
        }

        private string ApplyPepper(string password, string salt) => salt + password;

        private string HashPepperedPassword(string pepperedPassword)
        {
            var saltedPasswordBytes = Encoding.UTF8.GetBytes(pepperedPassword);
            using (var sha512 = SHA512.Create())
            {
                var hashedBytes = sha512.ComputeHash(saltedPasswordBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
            }
        }
    }
}
