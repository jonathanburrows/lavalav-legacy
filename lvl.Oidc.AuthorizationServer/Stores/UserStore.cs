using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    internal class UserStore
    {
        private IRepository<User> UserRepository { get; }
        private PasswordHasher PasswordHasher { get; }

        public UserStore(IRepository<User> userRepository, PasswordHasher passwordHasher)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            PasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var user = await FindByUsername(username) ?? throw new NullReferenceException("user");

            return PasswordHasher.VerifyHashedPassword(user.HashedPassword, password);
        }

        public async Task<User> FindByUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            var userQuery = new Query<User>().Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var users = await UserRepository.GetAsync(userQuery);
            if (users.Count == 0)
            {
                throw new InvalidOperationException($"No user was found with username '{username}'.");
            }
            if (users.Count > 1)
            {
                throw new InvalidOperationException($"{users.Count} users were found with the username '{username}'.");
            }

            return users.Items.Single();
        }
    }
}
