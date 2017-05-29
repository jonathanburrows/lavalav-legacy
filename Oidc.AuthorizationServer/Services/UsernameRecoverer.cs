using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    public class UsernameRecoverer
    {
        private IRepository<User> UserRepository { get; }

        public UsernameRecoverer(IRepository<User> userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Validates if a given email has any accounts with usernames.
        /// </summary>
        /// <param name="email">The email which may be associated to users.</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <returns>True if there exists at least one user with that email, false otherwise.</returns>
        public async Task<bool> AnyUsersHaveEmailAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            var emailQuery = new Query<User>().Where(user => user.Claims.Any(c => c.Type == JwtClaimTypes.Email && c.Value.ToLower() == email.ToLower()));
            var usersWithEmail = await UserRepository.GetAsync(emailQuery);

            return usersWithEmail.Items.Any();
        }

        /// <summary>
        ///     Sends an email with the corresponding username.
        /// </summary>
        /// <param name="recovering"></param>
        public async Task RecoverUsernameAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            var emailQuery = new Query<User>().Where(user => user.Claims.Any(c => c.Type == JwtClaimTypes.Email && c.Value.ToLower() == email.ToLower()));
            var usersWithEmail = await UserRepository.GetAsync(emailQuery);

            if (!usersWithEmail.Items.Any())
            {
                throw new InvalidOperationException($"No users have the email '{email}'.");
            }

            // TODO: create an email service.
        }
    }
}
