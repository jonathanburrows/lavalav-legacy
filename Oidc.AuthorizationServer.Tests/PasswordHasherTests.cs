using lvl.Oidc.AuthorizationServer.Services;
using System;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    public class PasswordHasherTests
    {
        private PasswordHasher PasswordHasher { get; }

        public PasswordHasherTests()
        {
            PasswordHasher = new PasswordHasher();
        }

        [Fact]
        public void Hash_password_will_throw_argument_null_exception_when_password_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Hash(null, ""));
        }

        [Fact]
        public void Hash_password_will_throw_argument_null_exception_when_salt_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Hash("", null));
        }

        [Fact]
        public void Hash_password_will_modify_given_password()
        {
            var password = "my-password";
            var salt = PasswordHasher.GetSalt();

            var hashed = PasswordHasher.Hash(password, salt);

            Assert.NotEqual(password, hashed);
        }

        [Fact]
        public void Hash_password_will_generate_different_passwords_with_different_salts()
        {
            var password = "my-password";
            var firstSalt = "my-first-salt";
            var secondSalt = "my-second-salt";

            var firstHashed = PasswordHasher.Hash(password, firstSalt);
            var secondHashed = PasswordHasher.Hash(password, secondSalt);

            Assert.NotEqual(firstHashed, secondHashed);
        }

        [Fact]
        public void Verify_hashed_password_will_throw_argument_null_exception_when_hashed_password_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Verify(null, "", ""));
        }

        [Fact]
        public void Verify_hashed_password_will_throw_argument_null_exception_when_provided_password_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Verify("", null, ""));
        }

        [Fact]
        public void Verify_hashed_passworD_will_throw_argument_null_exception_when_salt_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Verify("", "", null));
        }

        [Fact]
        public void Verify_will_be_false_when_hashed_equals_provided_exactly()
        {
            var password = "password";
            var salt = PasswordHasher.GetSalt();

            Assert.False(PasswordHasher.Verify(password, password, salt));
        }

        [Fact]
        public void Verify_will_be_false_when_hashed_is_provided_with_different_salt()
        {
            var password = "password";
            var firstSalt = "first-salt";
            var secondSalt = "second-salt";
            var hashedPassword = PasswordHasher.Hash(password, firstSalt);

            Assert.False(PasswordHasher.Verify(hashedPassword, password, secondSalt));
        }

        [Fact]
        public void Verify_will_be_false_when_hashed_does_not_equal_provided_with_same_salt()
        {
            var password = "correct-password";
            var salt = PasswordHasher.GetSalt();
            var hashedPassword = PasswordHasher.Hash(password, salt);

            Assert.False(PasswordHasher.Verify(hashedPassword, salt, "incorrect-password"));
        }

        [Fact]
        public void Verify_will_be_true_when_hashed_is_provided_with_same_salt()
        {
            var password = "password";
            var salt = PasswordHasher.GetSalt();
            var hashedPassword = PasswordHasher.Hash(password, salt);

            Assert.True(PasswordHasher.Verify(hashedPassword, password, salt));
        }
    }
}