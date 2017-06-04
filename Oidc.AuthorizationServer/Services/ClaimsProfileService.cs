using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using lvl.Repositories;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories.Querying;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Will attach subject and requested claims to a userinfo request.
    /// </summary>
    internal class ClaimsProfileService : IProfileService
    {
        private IRepository<User> UserRepository { get; }

        public ClaimsProfileService(IRepository<User> userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Attaches subject and requested claims to a userinfo request.
        ///     
        ///     Will attach the sub claim.
        ///     
        ///     Will attach any claim's who's Type matches the requested scopes.
        /// </summary>
        /// <param name="context">The context who will have claims attached, and contains the user identity.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null.</exception>
        /// <exception cref="ArgumentException">context.RequestedClaimTypes is null.</exception>
        /// <exception cref="InvalidOperationException">There was no subject provided.</exception>
        /// <exception cref="InvalidOperationException">There was no matching user with the given subject.</exception>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.RequestedClaimTypes == null)
            {
                throw new ArgumentException($"{nameof(context.RequestedClaimTypes)} is null.");
            }

            var subject = context.Subject.FindFirst("sub")?.Value;
            if (subject == null)
            {
                throw new InvalidOperationException("User has no subject.");
            }

            var userQuery = new Query<User>().Where(u => u.SubjectId == subject);
            var users = await UserRepository.GetAsync(userQuery);
            var user = users.Items.SingleOrDefault();
            if (user == null)
            {
                throw new InvalidOperationException($"No user with subject {subject} was found.");
            }

            // Messy, but I'm trying to be consistent with IdentityServer4 source code conventions.
            var claims = user.Claims.Select(c => c.ToSecurityClaim());
            context.IssuedClaims.AddRange(context.FilterClaims(claims));

            context.IssuedClaims.AddRange(context.FilterClaims(context.Subject.Claims));
        }

        /// <summary>
        ///     Always considers the user to be active.
        /// </summary>
        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
