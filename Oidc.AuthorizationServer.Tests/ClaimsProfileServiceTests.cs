using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using static IdentityServer4.IdentityServerConstants.ProfileDataCallers;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class ClaimsProfileServiceTests
    {
        private IRepository<User> UserRepository { get; }
        private IProfileService ProfileService { get; }

        public ClaimsProfileServiceTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;

            UserRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            ProfileService = serviceProvider.GetRequiredService<IProfileService>();
        }

        [Fact]
        public async Task It_will_throw_argument_null_exception_when_context_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => ProfileService.GetProfileDataAsync(null));
        }

        [Fact]
        public async Task It_will_throw_argument_exception_when_requested_claim_types_are_null()
        {
            var claims = new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, "testuser@lavalav.com") });
            var context = new ProfileDataRequestContext(new ClaimsPrincipal(claims), new Client(), UserInfoEndpoint, new string[0])
            {
                RequestedClaimTypes = null
            };
            await Assert.ThrowsAsync<ArgumentException>(() => ProfileService.GetProfileDataAsync(context));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_if_no_subject_provided()
        {
            var context = new ProfileDataRequestContext(new ClaimsPrincipal(), new Client(), UserInfoEndpoint, new string[0]);

            await Assert.ThrowsAsync<InvalidOperationException>(() => ProfileService.GetProfileDataAsync(context));
        }

        [Fact]
        public async Task It_will_throw_invalid_operation_exception_when_user_does_not_exist()
        {
            var claims = new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, "non-existant") });
            var context = new ProfileDataRequestContext(new ClaimsPrincipal(claims), new Client(), UserInfoEndpoint, new string[0]);

            await Assert.ThrowsAsync<InvalidOperationException>(() => ProfileService.GetProfileDataAsync(context));
        }

        [Fact]
        public async Task It_shall_not_return_unrequested_claims()
        {
            const string username = nameof(It_shall_not_return_unrequested_claims);
            await UserRepository.CreateAsync(new User
            {
                Username = username,
                SubjectId = username,
                Claims = { new ClaimEntity { Type = JwtClaimTypes.Email, Value = "this email" } }
            });
            var claims = new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, username) });
            var context = new ProfileDataRequestContext(new ClaimsPrincipal(claims), new Client(), UserInfoEndpoint, new string[0]);

            await ProfileService.GetProfileDataAsync(context);

            Assert.False(context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Email));
        }

        [Fact]
        public async Task It_shall_return_requested_claims()
        {
            const string username = nameof(It_shall_return_requested_claims);
            await UserRepository.CreateAsync(new User
            {
                Username = username,
                SubjectId = username,
                Claims = { new ClaimEntity { Type = JwtClaimTypes.Email, Value = "this email" } }
            });
            var claims = new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, username) });
            var identity = new ClaimsPrincipal(claims);
            var requestedClaims = new[] { JwtClaimTypes.Email };
            var context = new ProfileDataRequestContext(identity, new Client(), UserInfoEndpoint, requestedClaims);

            await ProfileService.GetProfileDataAsync(context);

            Assert.True(context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Email));
        }

        [Fact]
        public async Task Is_active_will_return_true()
        {
            var context = new IsActiveContext(new ClaimsPrincipal(), new Client(), nameof(Is_active_will_return_true));

            await ProfileService.IsActiveAsync(context);

            Assert.True(context.IsActive);
        }
    }
}
