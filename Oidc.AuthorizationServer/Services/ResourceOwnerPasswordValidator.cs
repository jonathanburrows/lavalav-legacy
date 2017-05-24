using IdentityModel;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Handles validation of resource owner password credentials.
    /// </summary>
    internal class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserStore UserStore { get; }

        public ResourceOwnerPasswordValidator(UserStore userStore)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        /// <summary>
        ///     Will validate the provided credentials, and set the result.
        /// </summary>
        /// <param name="context">Contains the credentials and result of the login attempt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null.</exception>
        /// <returns>The given context, with an updated result.</returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = await ValidateCredentialsAsync(context);
        }

        private async Task<GrantValidationResult> ValidateCredentialsAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await UserStore.FindByUsernameAsync(context.UserName);
            if (user == null)
            {
                return new GrantValidationResult
                {
                    IsError = true,
                    ErrorDescription = "invalid_user",
                    Error = "Couldn't find user"
                };
            }
            else if (!await UserStore.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                return new GrantValidationResult
                {
                    IsError = true,
                    ErrorDescription = "invalid_password",
                    Error = "Incorrect password, try again"
                };
            }
            else
            {
                var claims = user.Claims.Select(claim => claim.ToSecurityClaim());
                return new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password, claims);
            }
        }
    }
}
