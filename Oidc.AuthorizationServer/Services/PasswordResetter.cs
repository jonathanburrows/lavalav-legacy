using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Generates reset password tokens, and changes passwords.
    /// </summary>
    public class PasswordResetter
    {
        private IRepository<ResetPasswordToken> TokenRepository { get; }
        private UserStore UserStore { get; }

        public PasswordResetter(IRepository<ResetPasswordToken> tokenRepository, UserStore userStore)
        {
            TokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        /// <summary>
        ///     Generates a password token and sends it to the user's registered email.
        /// </summary>
        /// <param name="username">The username of the user resetting their password.</param>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No user found with given username.</exception>
        /// <exception cref="InvalidOperationException">User has no email.</exception>
        public async Task SendResetPasswordEmail(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            var user = await UserStore.FindByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException($"No user found with username '{username}'");
            }
            if (!user.Claims.Any(c => c.Type == JwtClaimTypes.Email))
            {
                throw new InvalidOperationException($"'{username}' does not have a registered email.");
            }

            var passwordToken = new ResetPasswordToken
            {
                CreatedOn = DateTime.Now,
                Token = Guid.NewGuid(),
                UserId = user.Id
            };
            await TokenRepository.CreateAsync(passwordToken);

            // TODO: create an email sender.
        }
    }
}
