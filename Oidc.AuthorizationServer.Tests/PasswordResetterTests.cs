using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class PasswordResetterTests
    {
        private PasswordResetter PasswordResetter { get; }
        private UserStore UserStore { get; }
        private PasswordHasher PasswordHasher { get; }

        public PasswordResetterTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;

            PasswordResetter = serviceProvider.GetRequiredService<PasswordResetter>();
            UserStore = serviceProvider.GetRequiredService<UserStore>();
            PasswordHasher = serviceProvider.GetRequiredService<PasswordHasher>();
        }

        [Fact]
        public async Task Reset_will_throw_argument_null_exception_when_subject_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PasswordResetter.ResetPassword(null, "password"));
        }

        [Fact]
        public async Task Reset_will_throw_argument_null_exception_when_new_password_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PasswordResetter.ResetPassword("user", null));
        }

        [Fact]
        public async Task Reset_will_throw_invalid_operation_exception_when_user_does_not_exist()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => PasswordResetter.ResetPassword("non-existant-user", "password"));
        }

        [Fact]
        public async Task Reset_will_not_update_user_with_different_subject()
        {
            var updating = await UserStore.AddLocalUserAsync(Guid.NewGuid().ToString(), "old password");
            var notUpdating = await UserStore.AddLocalUserAsync(Guid.NewGuid().ToString(), "other-users-password");
            var notUpdatingOriginalPassword = notUpdating.HashedPassword;

            await PasswordResetter.ResetPassword(updating.SubjectId, "updating");

            Assert.Equal(notUpdating.HashedPassword, notUpdatingOriginalPassword);
        }

        [Fact]
        public async Task Reset_will_update_matching_users_password()
        {
            var user = await UserStore.AddLocalUserAsync(Guid.NewGuid().ToString(), "old password");
            var oldPassword = user.HashedPassword;

            await PasswordResetter.ResetPassword(user.SubjectId, "updated password");
            var updatedUser = await UserStore.FindBySubjectAsync(user.SubjectId);

            Assert.NotEqual(updatedUser.HashedPassword, oldPassword);
        }

        [Fact]
        public async Task Reset_will_hash_password()
        {
            var user = await UserStore.AddLocalUserAsync(Guid.NewGuid().ToString(), "old password");

            await PasswordResetter.ResetPassword(user.SubjectId, "updated password");
            var updatedUser = await UserStore.FindBySubjectAsync(user.SubjectId);

            Assert.Equal(updatedUser.HashedPassword, PasswordHasher.Hash("updated password", updatedUser.Salt));
        }
    }
}
