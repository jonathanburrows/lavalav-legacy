using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    public class UserStore
    {
        private IRepository<User> UserRepository { get; }
        private PasswordHasher PasswordHasher { get; }

        public UserStore(IRepository<User> userRepository, PasswordHasher passwordHasher)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            PasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var user = await FindByUsernameAsync(username) ?? throw new NullReferenceException("user");

            return PasswordHasher.VerifyHashedPassword(user.HashedPassword, password, user.Salt);
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            var userQuery = new Query<User>().Where(u => u.Username.ToLower() == username.ToLower());
            var users = await UserRepository.GetAsync(userQuery);
            if (users.Count > 1)
            {
                throw new InvalidOperationException($"{users.Count} users were found with the username '{username}'.");
            }

            return users.Items.SingleOrDefault();
        }

        public async Task<User> AddUserAsync(User adding)
        {
            adding.Salt = PasswordHasher.GetSalt();
            adding.HashedPassword = PasswordHasher.HashPassword(adding.HashedPassword, adding.Salt);
            return await UserRepository.CreateAsync(adding);
        }
    }
}
