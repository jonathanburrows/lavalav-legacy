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
        private PasswordHasher PasswordHasher { get; }
        private IRepository<User> UserRepository { get; }

        public PasswordResetter(IRepository<ResetPasswordToken> tokenRepository, UserStore userStore, PasswordHasher passwordHasher, IRepository<User> userRepository)
        {
            TokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            PasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Generates a password token and sends it to the user's registered email.
        /// </summary>
        /// <param name="username">The username of the user resetting their password.</param>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No user found with given username.</exception>
        /// <exception cref="InvalidOperationException">User has no email.</exception>
        public async Task SendResetPasswordEmailAsync(string username)
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
            if (user.Claims.All(c => c.Type != JwtClaimTypes.Email))
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

        /// <summary>
        ///     Will modify a user's password.
        /// </summary>
        /// <param name="subject">The identifier of the user being updated.</param>
        /// <param name="newPassword">The password which will be hashed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subject"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="newPassword"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No user was found with subject.</exception>
        /// <remarks>
        ///     Validation checks arent done here, to allow for administrators to change passwords.
        ///     Validation should be done in the layer above.
        /// </remarks>
        public async Task ResetPassword(string subject, string newPassword)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }
            if (newPassword == null)
            {
                throw new ArgumentNullException(nameof(newPassword));
            }

            var user = await UserStore.FindBySubjectAsync(subject);
            if(user == null)
            {
                throw new InvalidOperationException($"No user was found with subject '{subject}'");
            }

            user.HashedPassword = PasswordHasher.Hash(newPassword, user.Salt);
            await UserRepository.UpdateAsync(user);
        }
    }
}
