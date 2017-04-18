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

        public async Task<User> AddExternalUser(string provider, string userId, IEnumerable<Claim> externalClaims)
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

            var claims = TransformExternalClaims(externalClaims);

            var nameClaims = claims.Where(claim => claim.Type == JwtClaimTypes.Name);
            if (nameClaims.Count() > 1)
            {
                throw new InvalidOperationException($"{nameClaims.Count()} name claims found.");
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
