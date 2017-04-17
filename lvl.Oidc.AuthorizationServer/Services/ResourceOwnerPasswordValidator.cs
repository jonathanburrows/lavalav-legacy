using IdentityModel;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserStore UserStore { get; }

        public ResourceOwnerPasswordValidator(UserStore userStore)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
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
                    Error = "Couldn't find user"
                };
            }
            else if (!await UserStore.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                return new GrantValidationResult
                {
                    IsError = true,
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
