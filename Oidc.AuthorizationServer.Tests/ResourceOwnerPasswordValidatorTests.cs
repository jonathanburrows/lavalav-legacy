using IdentityModel;
using IdentityServer4.Validation;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class ResourceOwnerPasswordValidatorTests
    {
        private IResourceOwnerPasswordValidator ResourceOwnerPasswordValidator { get; }
        private UserStore UserStore { get; }

        public ResourceOwnerPasswordValidatorTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            ResourceOwnerPasswordValidator = serviceProvider.GetRequiredService<IResourceOwnerPasswordValidator>();
            UserStore = serviceProvider.GetRequiredService<UserStore>();
        }

        [Fact]
        public async Task It_will_throw_argument_null_exception_if_context_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ResourceOwnerPasswordValidator.ValidateAsync(null));
        }

        [Fact]
        public async Task It_will_set_result_to_error_if_user_doesnt_exist()
        {
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "non-existant",
                Password = "password"
            };

            await ResourceOwnerPasswordValidator.ValidateAsync(context);

            Assert.True(context.Result.IsError);
        }

        [Fact]
        public async Task It_will_set_result_to_error_if_password_is_incorrect()
        {
            await UserStore.AddLocalUserAsync("ro-incorrect-password", "password");
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "ro-incorrect-password",
                Password = "incorrect-password"
            };

            await ResourceOwnerPasswordValidator.ValidateAsync(context);

            Assert.True(context.Result.IsError);
        }

        [Fact]
        public async Task It_will_set_result_to_success_if_credentials_are_correct()
        {
            await UserStore.AddLocalUserAsync("ro-valid-credentials", "password");
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "ro-valid-credentials",
                Password = "password"
            };

            await ResourceOwnerPasswordValidator.ValidateAsync(context);

            Assert.False(context.Result.IsError);
        }

        [Fact]
        public async Task It_will_set_result_to_claims_if_credentials_are_correct()
        {
            await UserStore.AddUserAsync(new User {
                Username = "ro-adding-claims",
                SubjectId = "ro-adding-claims",
                HashedPassword = "password",
                Claims = new[]
                {
                    new ClaimEntity{ Type = JwtClaimTypes.Name, Value = "my-name" },
                    new ClaimEntity{ Type = JwtClaimTypes.Address, Value = "address" }
                }
            });
            var context = new ResourceOwnerPasswordValidationContext
            {
                UserName = "ro-adding-claims",
                Password = "password"
            };

            await ResourceOwnerPasswordValidator.ValidateAsync(context);

            var addressClaims = context.Result.Subject.Claims.SingleOrDefault(c => c.Type == JwtClaimTypes.Address);
            Assert.NotNull(addressClaims);
        }
    }
}
