using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Repositories;
using lvl.Repositories.Querying;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Stores
{
    /// <summary>
    ///     Performs operations on users, and allows for authentication.
    /// </summary>
    public class UserStore
    {
        private IRepository<User> UserRepository { get; }
        private PasswordHasher PasswordHasher { get; }

        public UserStore(IRepository<User> userRepository, PasswordHasher passwordHasher)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            PasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /// <summary>
        ///     Checks if the username/password is valid.
        /// </summary>
        /// <param name="username">The given name of the user.</param>
        /// <param name="password">The given password.</param>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Could not find user.</exception>
        /// <returns>true if the username/password is correct; otherwise false.</returns>
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

            var user = await FindByUsernameAsync(username) ?? throw new InvalidOperationException($"User '{username}' could not be found.");

            return PasswordHasher.Verify(user.HashedPassword, password, user.Salt);
        }

        /// <summary>
        ///     Returns a user with a matching username.
        /// </summary>
        /// <param name="username">The username which will be searched for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null.</exception>
        /// <exception cref="InvalidOperationException">More than one user was found with that username.</exception>
        /// <returns>
        ///     A user with a matching username. If no user found, null is returned.
        /// </returns>
        /// <remarks>The search is case insensitive.</remarks>
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

        /// <summary>
        ///     Hases a user's password, then stores it in the repository.
        /// </summary>
        /// <param name="adding">The user to be stored.</param>
        /// <exception cref="ArgumentNullException"><paramref name="adding"/> is null.</exception>
        /// <returns>The given user with generated id.</returns>
        public async Task<User> AddUserAsync(User adding)
        {
            if(adding == null)
            {
                throw new ArgumentNullException(nameof(adding));
            }

            adding.Salt = PasswordHasher.GetSalt();
            adding.HashedPassword = PasswordHasher.Hash(adding.HashedPassword, adding.Salt);
            return await UserRepository.CreateAsync(adding);
        }

        /// <summary>
        ///     Returns a user from an external provider.
        /// </summary>
        /// <param name="provider">The name of the provider being requested.</param>
        /// <param name="userId">The subject id which will be sent to the external provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="userId"/> is null.</exception>
        /// <exception cref="InvalidOperationException">More than one user was found with that userId.</exception>
        /// <returns>The matching user. If no user was found, null is returned.</returns>
        public async Task<User> FindByExternalProviderAsync(string provider, string userId)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var query = new Query<User>()
                .Where(user => user.ProviderName == provider)
                .Where(user => user.ProviderSubjectId == userId);
            var users = await UserRepository.GetAsync(query);

            if (users.Count > 1)
            {
                throw new InvalidOperationException($"{users.Count} users found for provider '{provider}' with the user id '{userId}'");
            }

            return users.Items.SingleOrDefault();
        }

        /// <summary>
        ///     Adds a user which will be authenticated by the oidc server.
        /// </summary>
        /// <param name="username">The username which will be given to the added user.</param>
        /// <param name="password">The password which will be hashed and stored.</param>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Username is already taken.</exception>
        /// <returns>The created user.</returns>
        public async Task<User> AddLocalUserAsync(string username, string password)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username), "Please give a username");
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "Please give a password");
            }

            var matchingUsersQuery = new Query<User>().Where(u => u.Username.ToLower() == username.ToLower());
            var matchingUsers = await UserRepository.GetAsync(matchingUsersQuery);
            if (matchingUsers.Count > 0)
            {
                throw new InvalidOperationException("Username already taken");
            }

            var claims = new[] { new ClaimEntity { Type = JwtClaimTypes.Name, Value = username } };

            var salt = PasswordHasher.GetSalt();
            var hashedPassword = PasswordHasher.Hash(password, salt);

            var user = new User
            {
                SubjectId = username,
                Salt = salt,
                HashedPassword = hashedPassword,
                Username = username,
                Claims = claims
            };
            await UserRepository.CreateAsync(user);

            return user;
        }

        /// <summary>
        ///     Will create a user for an account authenticated by a external provider (like facebook).
        /// </summary>
        /// <param name="provider">The provider which authenticate the user.</param>
        /// <param name="userId">The unique (for the provider) identifier of the user</param>
        /// <param name="externalClaims">Information about the identity of the user.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="userId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="externalClaims"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Multiple names were specified for the user.</exception>
        /// <returns>The created user.</returns>
        public async Task<User> AddExternalUserAsync(string provider, string userId, IEnumerable<Claim> externalClaims)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (externalClaims == null)
            {
                throw new ArgumentNullException(nameof(externalClaims));
            }

            var subjectId = CryptoRandom.CreateUniqueId();

            var claims = TransformExternalClaims(externalClaims).ToList();

            var nameClaims = claims.Where(claim => claim.Type == JwtClaimTypes.Name).ToList();
            if (nameClaims.Count > 1)
            {
                throw new InvalidOperationException($"{nameClaims.Count} name claims found.");
            }
            var username = nameClaims.FirstOrDefault()?.Value ?? subjectId;

            var claimEntities = claims.Select(c => new ClaimEntity
            {
                Issuer = c.Issuer,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            }).ToList();

            var externalUser = new User
            {
                SubjectId = subjectId,
                Username = username,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = claimEntities
            };

            await UserRepository.CreateAsync(externalUser);

            return externalUser;
        }

        /// <summary>
        ///     Will clean up claims given by 3rd parties and makes them consumable by this oidc server.
        /// </summary>
        /// <param name="externalClaims">The claims provided by the 3rd party provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="externalClaims"/> is null.</exception>
        /// <returns>The valid set of claims.</returns>
        private IEnumerable<Claim> TransformExternalClaims(IEnumerable<Claim> externalClaims)
        {
            if (externalClaims == null)
            {
                throw new ArgumentNullException(nameof(externalClaims));
            }

            foreach (var claim in externalClaims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    yield return new Claim(JwtClaimTypes.Name, claim.Value);
                }
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    var outboundClaimType = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type];
                    yield return new Claim(outboundClaimType, claim.Value);
                }
                else
                {
                    yield return claim;
                }
            }
        }
    }
}
