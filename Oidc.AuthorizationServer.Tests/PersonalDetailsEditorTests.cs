using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Oidc.AuthorizationServer.ViewModels;
using lvl.Repositories;
using lvl.Web.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class PersonalDetailsEditorTests
    {
        private IRepository<User> UserRepository { get; }
        private PersonalDetailsEditor PersonalDetailsEditor { get; }
        private Impersonator Impersonator { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public PersonalDetailsEditorTests(ServiceProviderFixture serviceProviderFixture)
        {
            var services = serviceProviderFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

            UserRepository = services.GetRequiredService<IRepository<User>>();
            PersonalDetailsEditor = services.GetRequiredService<PersonalDetailsEditor>();
            Impersonator = services.GetRequiredService<Impersonator>();
            HttpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
        }

        [Fact]
        public async Task Get_current_user_will_throw_invalid_operation_exception_if_http_context_is_null()
        {
            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<InvalidOperationException>(() => PersonalDetailsEditor.GetCurrentUserAsync());
        }

        [Fact]
        public async Task Get_current_user_will_throw_invalid_operation_exception_if_user_is_null()
        {
            HttpContextAccessor.HttpContext = new DefaultHttpContext();

            await Assert.ThrowsAsync<InvalidOperationException>(() => PersonalDetailsEditor.GetCurrentUserAsync());
        }

        [Fact]
        public async Task Get_current_user_will_throw_invalid_operation_exception_if_user_is_not_authenticated()
        {
            var claims = new ClaimsIdentity();
            HttpContextAccessor.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(claims)
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => PersonalDetailsEditor.GetCurrentUserAsync());
        }

        [Fact]
        public async Task Get_current_user_will_throw_invalid_operation_exception_if_user_has_no_sub_claim()
        {
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("name", "no-sub")
            }, "impersonated", "name", "role");
            HttpContextAccessor.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(claims)
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => PersonalDetailsEditor.GetCurrentUserAsync());
        }

        [Fact]
        public async Task Get_current_user_will_throw_invalid_operation_exception_if_no_matching_user()
        {
            Impersonator.AsUser("non-existant-user");

            await Assert.ThrowsAsync<InvalidOperationException>(() => PersonalDetailsEditor.GetCurrentUserAsync());
        }

        [Fact]
        public async Task Get_current_user_will_return_matching_user()
        {
            var name = nameof(Get_current_user_will_return_matching_user);
            var user = await UserRepository.CreateAsync(new User
            {
                SubjectId = name,
                Username = name,
                Claims = new[] { new ClaimEntity { Type = "sub", Value = name } }
            });
            Impersonator.AsUser(name);

            var matchingUser = await PersonalDetailsEditor.GetCurrentUserAsync();

            Assert.Equal(user, matchingUser);
        }

        [Fact]
        public async Task Get_current_user_will_not_return_user_with_different_sub()
        {
            var differentName = nameof(Get_current_user_will_not_return_user_with_different_sub) + "--different";
            var matchingName = nameof(Get_current_user_will_not_return_user_with_different_sub);
            await UserRepository.CreateAsync(new User
            {
                SubjectId = differentName,
                Username = differentName,
                Claims = new[] { new ClaimEntity { Type = "sub", Value = differentName } }
            });
            var user = await UserRepository.CreateAsync(new User
            {
                SubjectId = matchingName,
                Username = matchingName,
                Claims = new[] { new ClaimEntity { Type = "sub", Value = matchingName } }
            });
            Impersonator.AsUser(matchingName);

            var matchingUser = await PersonalDetailsEditor.GetCurrentUserAsync();

            Assert.Equal(user, matchingUser);
        }

        [Fact]
        public void Get_personal_details_will_throw_argument_null_exception_if_user_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => PersonalDetailsEditor.GetPersonalDetails(null));
        }

        [Fact]
        public void Get_personal_details_will_return_empty_model_if_claims_are_null()
        {
            var user = new User();

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.NotNull(model);
        }

        [Fact]
        public void Get_personal_details_will_populate_email()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.Email, Value = "my-email" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.Email, "my-email");
        }

        [Fact]
        public void Get_personal_details_will_populate_first_name()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.GivenName, Value = "my-first-name" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.FirstName, "my-first-name");
        }

        [Fact]
        public void Get_personal_details_will_populate_last_name()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.FamilyName, Value = "my-last-name" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.LastName, "my-last-name");
        }

        [Fact]
        public void Get_personal_details_will_populate_job()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = "job", Value = "my-job" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.Job, "my-job");
        }

        [Fact]
        public void Get_personal_details_will_populate_location()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = "location", Value = "my-location" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.Location, "my-location");
        }

        [Fact]
        public void Get_personal_details_will_populate_phone()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.PhoneNumber, Value = "1234567" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Equal(model.PhoneNumber, 1234567);
        }

        [Fact]
        public void Get_personal_details_will_populate_empty_phone_to_null()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.PhoneNumber, Value = "" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Null(model.PhoneNumber);
        }

        [Fact]
        public void Get_personal_details_will_populate_non_number_phone_to_null()
        {
            var user = new User
            {
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.PhoneNumber, Value = "HELLO, WORLD!" } }
            };

            var model = PersonalDetailsEditor.GetPersonalDetails(user);

            Assert.Null(model.PhoneNumber);
        }

        [Fact]
        public async Task Update_will_throw_argument_null_exception_if_user_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(null, new PersonalDetailsViewModel()));
        }

        [Fact]
        public async Task Update_will_throw_argument_null_exception_if_personal_details_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(new User(), null));
        }

        [Fact]
        public async Task Update_will_add_email_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_email_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Email = "my-email" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.Email);
            Assert.Equal(claim.Value, "my-email");
        }

        [Fact]
        public async Task Update_will_update_email_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_email_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.Email, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Email = "updated" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.Email);
            Assert.Equal(claim.Value, "updated");
        }

        [Fact]
        public async Task Update_will_remove_email_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_email_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.Email, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.Email));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_email_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_email_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.Email));
        }

        [Fact]
        public async Task Update_will_add_first_name_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_first_name_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { FirstName = "my-first-name" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.GivenName);
            Assert.Equal(claim.Value, "my-first-name");
        }

        [Fact]
        public async Task Update_will_update_first_name_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_first_name_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.GivenName, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { FirstName = "updated" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.GivenName);
            Assert.Equal(claim.Value, "updated");
        }

        [Fact]
        public async Task Update_will_remove_first_name_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_first_name_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.GivenName, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { FirstName = "" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.GivenName));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_first_name_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_first_name_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.Email));
        }

        [Fact]
        public async Task Update_will_add_last_name_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_last_name_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { LastName = "my-last-name" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.FamilyName);
            Assert.Equal(claim.Value, "my-last-name");
        }

        [Fact]
        public async Task Update_will_update_last_name_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_last_name_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.FamilyName, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { LastName = "updated" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.FamilyName);
            Assert.Equal(claim.Value, "updated");
        }

        [Fact]
        public async Task Update_will_remove_last_name_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_last_name_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.FamilyName, Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.FamilyName));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_last_name_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_last_name_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.FamilyName));
        }

        [Fact]
        public async Task Update_will_add_phone_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_phone_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { PhoneNumber = 1 };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.PhoneNumber);
            Assert.Equal(claim.Value, "1");
        }

        [Fact]
        public async Task Update_will_update_phone_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_phone_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.PhoneNumber, Value = "1" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { PhoneNumber = 2 };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == JwtClaimTypes.PhoneNumber);
            Assert.Equal(claim.Value, "2");
        }

        [Fact]
        public async Task Update_will_remove_phone_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_phone_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = JwtClaimTypes.PhoneNumber, Value = "1" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.PhoneNumber));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_phone_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_phone_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == JwtClaimTypes.PhoneNumber));
        }

        [Fact]
        public async Task Update_will_add_job_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_job_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Job = "my-job" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == "job");
            Assert.Equal(claim.Value, "my-job");
        }

        [Fact]
        public async Task Update_will_update_job_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_job_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = "job", Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Job = "updated" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == "job");
            Assert.Equal(claim.Value, "updated");
        }

        [Fact]
        public async Task Update_will_remove_job_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_job_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = "job", Value = "value" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == "job"));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_job_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_job_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == "job"));
        }

        [Fact]
        public async Task Update_will_add_location_if_in_model_but_not_claims()
        {
            var username = nameof(Update_will_add_location_if_in_model_but_not_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Location = "my-location" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == "location");
            Assert.Equal(claim.Value, "my-location");
        }

        [Fact]
        public async Task Update_will_update_location_if_in_model_and_claims()
        {
            var username = nameof(Update_will_update_location_if_in_model_and_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = "location", Value = "old" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel { Location = "updated" };

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            var claim = user.Claims.Single(c => c.Type == "location");
            Assert.Equal(claim.Value, "updated");
        }

        [Fact]
        public async Task Update_will_remove_location_if_not_in_model_but_in_claims()
        {
            var username = nameof(Update_will_remove_location_if_not_in_model_but_in_claims);
            var user = new User
            {
                Username = username,
                SubjectId = username,
                Claims = new[] { new ClaimEntity { Type = "location", Value = "value" } }
            };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == "location"));
        }

        [Fact]
        public async Task Update_will_not_modify_claims_if_location_not_in_model_or_claims()
        {
            var username = nameof(Update_will_not_modify_claims_if_location_not_in_model_or_claims);
            var user = new User { Username = username, SubjectId = username };
            await UserRepository.CreateAsync(user);
            var model = new PersonalDetailsViewModel();

            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);

            Assert.False(user.Claims.Any(c => c.Type == "location"));
        }
    }
}
