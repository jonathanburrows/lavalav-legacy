using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;

namespace lvl.Oidc.AuthorizationServer.Services
{
    public class PasswordHasher
    {
        private const int SaltLength = 16;
        private OidcAuthorizationServerOptions Options { get; }

        public PasswordHasher(IOptions<OidcAuthorizationServerOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public string HashPassword(string password, string salt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var pepperedPassword = ApplyPepper(password, salt);
            return HashPepperedPassword(pepperedPassword);
        }

        public string GetSalt()
        {
            var salt = new byte[SaltLength * 2];
            using (var saltGenerator = RandomNumberGenerator.Create())
            {
                saltGenerator.GetBytes(salt);
                return BitConverter.ToString(salt).Replace("-", string.Empty);
            }
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword, string salt)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
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
