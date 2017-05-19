using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(OidcAuthorizationServerCollection.Name)]
    public class UserStoreTests
    {
        private UserStore UserStore { get; }
        private IRepository<User> UserRepository { get; }
        private PasswordHasher PasswordHasher { get; }

        public UserStoreTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            UserStore = serviceProvider.GetRequiredService<UserStore>();
            UserRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            PasswordHasher = serviceProvider.GetRequiredService<PasswordHasher>();
        }

        [Fact]
        public async Task Validate_credentials_will_throw_argument_null_exception_when_username_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.ValidateCredentialsAsync(null, ""));
        }

        [Fact]
        public async Task Validate_credentials_will_throw_argument_null_exception_when_password_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.ValidateCredentialsAsync("", null));
        }

        [Fact]
        public async Task Validate_credentials_will_throw_invalid_argument_exception_when_no_matching_user()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => UserStore.ValidateCredentialsAsync("non-existant-user", ""));
        }

        [Fact]
        public async Task Validate_credentials_will_return_false_when_password_is_different()
        {
            var salt = PasswordHasher.GetSalt();
            var user = new User
            {
                Username = "my-user",
                HashedPassword = PasswordHasher.Hash("password", salt),
                Salt = salt,
                SubjectId = "my-user"
            };
            await UserRepository.CreateAsync(user);


            var isValid = await UserStore.ValidateCredentialsAsync("my-user", "incorrect-password");
            Assert.False(isValid);
        }

        [Fact]
        public async Task Validate_credentials_will_return_true_when_password_matches()
        {
            var salt = PasswordHasher.GetSalt();
            var user = new User
            {
                Username = "valid-user",
                HashedPassword = PasswordHasher.Hash("password", salt),
                Salt = salt,
                SubjectId = "valid-user"
            };
            await UserRepository.CreateAsync(user);


            var isValid = await UserStore.ValidateCredentialsAsync("valid-user", "password");
            Assert.True(isValid);
        }

        [Fact]
        public async Task Find_by_username_throws_argument_null_exception_when_username_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.FindByUsernameAsync(null));
        }

        [Fact]
        public async Task Find_by_username_will_return_null_when_no_matching_user_is_found()
        {
            var user = await UserStore.FindByUsernameAsync("no-existant-user");

            Assert.Null(user);
        }

        [Fact]
        public async Task Find_by_username_will_return_matching_user()
        {
            var user = new User
            {
                Username = "matching-user",
                SubjectId = "matching-user"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByUsernameAsync(user.Username);

            Assert.Equal(user, matchingUser);
        }

        [Fact]
        public async Task Find_by_username_will_return_matching_user_when_mismatched_case()
        {
            var user = new User
            {
                Username = "case-insensitive-user",
                SubjectId = "case-insensitive-user"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByUsernameAsync("CASE-INSENSITIVE-USER");

            Assert.Equal(user, matchingUser);
        }

        [Fact]
        public async Task Find_by_external_provider_will_throw_argument_null_exception_when_provider_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.FindByExternalProviderAsync(null, ""));
        }

        [Fact]
        public async Task Find_by_external_provider_will_throw_argument_null_exception_when_userId_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.FindByExternalProviderAsync("", null));
        }

        [Fact]
        public async Task Find_by_external_provider_will_return_null_if_no_matching_user()
        {
            var user = await UserStore.FindByExternalProviderAsync("random-provider", "non-existant-user");

            Assert.Null(user);
        }

        [Fact]
        public async Task Find_by_external_provider_Will_return_null_if_provider_matches_but_not_provider_subject_id()
        {
            var user = new User
            {
                Username = "provider-match-provider-subject-mismatch",
                SubjectId = "provider-match-provider-subject-mismatch",
                ProviderSubjectId = "lvl:provider-match-provider-subject-mismatch",
                ProviderName = "lvl"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByExternalProviderAsync("lvl:provider-match-provider-subject-mismatch", "mistmatched");

            Assert.Null(matchingUser);
        }

        [Fact]
        public async Task Find_by_external_provider_Will_return_null_if_provider_subject_id_matches_but_not_provider()
        {
            var user = new User
            {
                Username = "provider-subject-match-provider-mismatch",
                SubjectId = "provider-subject-match-provider-mismatch",
                ProviderSubjectId = "lvl:provider-subject-match-provider-mismatch",
                ProviderName = "lvl"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByExternalProviderAsync("mismatch", "provider-subject-match-provider-mismatch");

            Assert.Null(matchingUser);
        }

        [Fact]
        public async Task Find_by_external_provider_Will_return_null_if_provider_matches_but_local_subject_id_matches()
        {
            var user = new User
            {
                Username = "provider-match-but-uses-local",
                SubjectId = "provider-match-but-uses-local",
                ProviderSubjectId = "lvl:provider-match-but-uses-local",
                ProviderName = "lvl"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByExternalProviderAsync("provider-match-but-uses-local", "lvl");

            Assert.Null(matchingUser);
        }

        [Fact]
        public async Task Find_by_external_provider_Will_return_user_when_provider_and_provider_subject_id_matches()
        {
            var user = new User
            {
                Username = "matching-external-user",
                SubjectId = "matching-external-user",
                ProviderSubjectId = "lvl:matching-external-user",
                ProviderName = "lvl"
            };
            await UserRepository.CreateAsync(user);

            var matchingUser = await UserStore.FindByExternalProviderAsync("lvl", "lvl:matching-external-user");

            Assert.NotNull(matchingUser);
        }

        [Fact]
        public async Task Add_local_user_throws_argument_null_exception_if_username_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddLocalUserAsync(null, ""));
        }

        [Fact]
        public async Task Add_local_user_throws_argument_null_exception_if_password_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddLocalUserAsync("", null));
        }

        [Fact]
        public async Task Add_local_user_throws_invalid_operation_exception_if_username_already_taken()
        {
            await UserRepository.CreateAsync(new User
            {
                Username = "already-taken",
                SubjectId = "already-taken"
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => UserStore.AddLocalUserAsync("already-taken", "password"));
        }

        [Fact]
        public async Task Add_local_user_will_persist_the_user()
        {
            var persistedUser = await UserStore.AddLocalUserAsync("persisted-user", "password");

            var matchingUser = await UserRepository.GetAsync(persistedUser.Id);

            Assert.NotNull(matchingUser);
        }

        [Fact]
        public async Task Add_local_user_will_set_the_subject_id_to_username()
        {
            var username = "setting-subject-id";
            var user = await UserStore.AddLocalUserAsync(username, "password");

            Assert.Equal(user.SubjectId, username);
        }

        [Fact]
        public async Task Add_local_user_will_set_username_to_username()
        {
            var username = "setting-username";
            var user = await UserStore.AddLocalUserAsync(username, "password");

            Assert.Equal(user.Username, username);
        }

        [Fact]
        public async Task Add_local_user_will_make_name_claim_with_username()
        {
            var username = "setting-claims";
            var user = await UserStore.AddLocalUserAsync(username, "password");

            Assert.True(user.Claims.Any(c => c.Value == username));
        }

        [Fact]
        public async Task Add_local_user_hashes_password()
        {
            var password = "password";
            var user = await UserStore.AddLocalUserAsync("hashing-password", password);

            var hashedPassword = PasswordHasher.Hash(password, user.Salt);

            Assert.Equal(user.HashedPassword, hashedPassword);
        }

        [Fact]
        public async Task Add_external_user_will_throw_argument_null_exception_when_provider_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddExternalUserAsync(null, "", new Claim[0]));
        }

        [Fact]
        public async Task Add_external_user_will_throw_argument_null_exception_when_user_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddExternalUserAsync("", null, new Claim[0]));
        }

        [Fact]
        public async Task Add_external_user_will_throw_argument_null_exception_when_claims_are_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserStore.AddExternalUserAsync("", "", null));
        }

        [Fact]
        public async Task Add_external_user_will_persist_user()
        {
            var user = await UserStore.AddExternalUserAsync("lvl", "external-persistence", new Claim[0]);

            var matchingUser = await UserRepository.GetAsync(user.Id);

            Assert.NotNull(matchingUser);
        }

        [Fact]
        public async Task Add_external_user_will_throw_invalid_operation_exception_when_multiple_name_claims_are_given()
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Name, "john"),
                new Claim(ClaimTypes.Name, "john")
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => UserStore.AddExternalUserAsync("lvl", "multiple-name-claim", claims));
        }

        [Fact]
        public async Task Add_external_user_will_set_username_to_name_claim_if_given()
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Name, "john")
            };
            var user = await UserStore.AddExternalUserAsync("lvl", "name-claim", claims);

            Assert.Equal(user.Username, "john");
        }

        [Fact]
        public async Task Add_external_user_will_set_username_to_soap_name_clam_if_given()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "john")
            };
            var user = await UserStore.AddExternalUserAsync("lvl", "soap-name-claim", claims);

            Assert.Equal(user.Username, "john");
        }

        [Fact]
        public async Task Add_external_user_will_add_claims_to_user()
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Name, "john"),
                new Claim(JwtClaimTypes.Address, "555 street"),
            };
            var user = await UserStore.AddExternalUserAsync("lvl", "external-adding-claims", claims);

            Assert.Equal(user.Claims.Count, 2);
        }

        [Fact]
        public async Task Add_external_user_will_set_provider_name_to_given_provider()
        {
            var user = await UserStore.AddExternalUserAsync("lvl", "external-setting-provider-name", new Claim[0]);

            Assert.Equal(user.ProviderName, "lvl");
        }
    }
}
