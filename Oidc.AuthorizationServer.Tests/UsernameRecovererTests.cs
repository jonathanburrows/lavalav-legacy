using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class UsernameRecovererTests
    {
        private UsernameRecoverer UsernameRecoverer { get; }
        private IRepository<User> UserRepository { get; }

        public UsernameRecovererTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;

            UsernameRecoverer = serviceProvider.GetRequiredService<UsernameRecoverer>();
            UserRepository = serviceProvider.GetRequiredService<IRepository<User>>();
        }

        [Fact]
        public async Task Any_users_have_email_will_throw_argument_null_exception_if_email_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UsernameRecoverer.RecoverUsernameAsync(null));
        }

        [Fact]
        public async Task Any_users_have_email_will_return_false_if_claims_are_null()
        {
            var email = nameof(Any_users_have_email_will_return_false_if_claims_are_null);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = null
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.False(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_false_if_no_claims()
        {
            var email = nameof(Any_users_have_email_will_return_false_if_no_claims);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new ClaimEntity[0]
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.False(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_false_if_zero_emails_match_exactly()
        {
            var email = nameof(Any_users_have_email_will_return_false_if_zero_emails_match_exactly);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new[]
                {
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = "other-email" }
                }
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.False(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_true_if_one_email_matches_exactly()
        {
            var email = nameof(Any_users_have_email_will_return_true_if_one_email_matches_exactly);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new[]
                {
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = email },
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = "other-email" }
                }
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.True(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_true_if_two_Emails_match_exactly()
        {
            var email = nameof(Any_users_have_email_will_return_true_if_two_Emails_match_exactly);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new[]
                {
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = email },
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = email }
                }
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.True(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_false_if_non_email_matches()
        {
            var email = nameof(Any_users_have_email_will_return_false_if_non_email_matches);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new[]
                {
                    new ClaimEntity { Type = JwtClaimTypes.PreferredUserName, Value = email }
                }
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.False(userHasEmail);
        }

        [Fact]
        public async Task Any_users_have_email_will_return_true_if_case_doesnt_match()
        {
            var email = nameof(Any_users_have_email_will_return_true_if_case_doesnt_match);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Claims = new[]
                {
                    new ClaimEntity { Type = JwtClaimTypes.Email, Value = email.ToUpper() }
                }
            });

            var userHasEmail = await UsernameRecoverer.AnyUsersHaveEmailAsync(email);

            Assert.True(userHasEmail);
        }
    }
}
