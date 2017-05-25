using lvl.Web.Authorization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class ImpersonationTests
    {
        private Impersonator Impersonator { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public ImpersonationTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            var serviceProvider = webServiceProviderFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));
            Impersonator = serviceProvider.GetRequiredService<Impersonator>();
            HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        }

        [Fact]
        public void Impersonate_administrator_will_set_name_to_administrator()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsAdministrator();

            var username = HttpContextAccessor.HttpContext.User.Identity.Name;
            Assert.Equal(username, "administrator");
        }

        [Fact]
        public void Impersonate_administrator_will_set_subject_to_administrator()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsAdministrator();

            var claims = (ClaimsIdentity)HttpContextAccessor.HttpContext.User.Identity;
            var subject = claims.FindFirst("sub").Value;
            Assert.Equal(subject, "administrator");
        }

        [Fact]
        public void Impersonate_administrator_will_set_role_to_administrator()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsAdministrator();

            Assert.True(HttpContextAccessor.HttpContext.User.IsInRole("administrator"));
        }

        [Fact]
        public void Impersonate_administrator_will_be_authenticated()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsAdministrator();

            Assert.True(HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated);
        }

        [Fact]
        public void Impersonate_user_will_set_username()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsUser("my-user");

            var username = HttpContextAccessor.HttpContext.User.Identity.Name;
            Assert.Equal(username, "my-user");
        }

        [Fact]
        public void Impersonate_user_will_set_subject()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsUser("my-user");

            var claims = (ClaimsIdentity)HttpContextAccessor.HttpContext.User.Identity;
            var subject = claims.FindFirst("sub").Value;
            Assert.Equal(subject, "my-user");
        }

        [Fact]
        public void Impersonate_user_will_not_set_role()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsUser("my-user");

            var claims = (ClaimsIdentity)HttpContextAccessor.HttpContext.User.Identity;
            Assert.Empty(claims.FindAll("role"));
        }

        [Fact]
        public void Impersonate_user_will_be_authenticated()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.AsUser("my-user");

            Assert.True(HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated);
        }

        [Fact]
        public void With_role_will_add_role_to_already_signed_in_user()
        {
            Impersonator.AsUser("my-user").WithRole("special-role");

            Assert.True(HttpContextAccessor.HttpContext.User.IsInRole("special-role"));
        }

        [Fact]
        public void With_role_will_not_override_signed_in_username()
        {
            Impersonator.AsUser("my-user").WithRole("special-role");

            Assert.Equal(HttpContextAccessor.HttpContext.User.Identity.Name, "my-user");
        }

        [Fact]
        public void With_role_will_create_user_if_none_signed_in()
        {
            HttpContextAccessor.HttpContext = null;

            Impersonator.WithRole("special-role");

            var user = HttpContextAccessor.HttpContext.User;
            Assert.NotNull(user);
        }

        [Fact]
        public void With_role_will_create_authenticated_user_if_none_signed_in()
        {
            Impersonator.WithRole("special-role");

            Assert.True(HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated);
        }

        [Fact]
        public void With_role_will_create_user_with_role_if_none_signed_in()
        {
            Impersonator.WithRole("special-role");

            Assert.True(HttpContextAccessor.HttpContext.User.IsInRole("special-role"));
        }
    }
}
