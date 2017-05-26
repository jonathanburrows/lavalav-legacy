using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace lvl.Web.Authorization
{
    /// <summary>
    ///     Modifies the credentials of a user to have a role/userid, so certain system functions can be performed.
    /// </summary>
    public class Impersonator
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public Impersonator(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        ///     Sets the current user to an administrator.
        /// </summary>
        /// <returns>The impersonator for chaining.</returns>
        public Impersonator AsAdministrator()
        {
            SignInAs("administrator");
            WithRole("administrator");

            return this;
        }

        /// <summary>
        ///     Sets the current user to a given user, with their roles and claims.
        /// </summary>
        /// <param name="userId">The identifier of the user, equal to the subject.</param>
        /// <remarks>Userid is equivilant to the subject.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="userId"/> is null.</exception>
        /// <returns>The impersonator for chaining.</returns>
        public Impersonator AsUser(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            SignInAs(userId);

            return this;
        }

        /// <summary>
        ///     Adds a role to the current user. If no user, one is instantiated.
        /// </summary>
        /// <param name="role">The role to be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="role"/> is null.</exception>
        /// <remarks>If no user is signed in, then they are given a temporary name.</remarks>
        /// <returns>The impersonator for chaining.</returns>
        public Impersonator WithRole(string role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            // If there is no user, they are signed in as a temporary user.
            if(HttpContextAccessor.HttpContext?.User == null)
            {
                var username = Guid.NewGuid().ToString();
                SignInAs(username);
            }

            // ReSharper disable PossibleNullReferenceException calling SignInAs ensures user/identity is not null.
            var claims = (ClaimsIdentity)HttpContextAccessor.HttpContext.User.Identity;
            claims.AddClaim(new Claim("role", role));

            return this;
        }

        private void SignInAs(string userId)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("name", userId),
                new Claim("sub", userId)
            }, "impersonated", "name", "role");

            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = claimsPrinciple;
        }
    }
}
