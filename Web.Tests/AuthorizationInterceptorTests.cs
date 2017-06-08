using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Repositories;
using lvl.Web.Authorization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Exceptions;
using System;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class AuthorizationInterceptorTests
    {
        private RepositoryFactory RepositoryFactory { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private Impersonator Impersonator { get; }

        public AuthorizationInterceptorTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            var serviceProvider = webServiceProviderFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));

            RepositoryFactory = serviceProvider.GetRequiredService<RepositoryFactory>();
            HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            Impersonator = serviceProvider.GetRequiredService<Impersonator>();
        }

        [Fact]
        public async Task When_creating_if_role_and_user_not_logged_in_an_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleButNoCreate>();

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleButNoCreate()));
        }

        [Fact]
        public async Task When_creating_if_role_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleButNoCreate>();

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "malicious user"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor?.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleButNoCreate()));
        }

        [Fact]
        public async Task When_creating_if_role_and_in_role_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<RoleButNoCreate>();

            Impersonator.AsUser("user").WithRole("other-role");
            var entity = await repository.CreateAsync(new RoleButNoCreate());

            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_if_role_and_not_in_role_but_administrator_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<RoleButNoCreate>();

            Impersonator.AsAdministrator();
            var entity = await repository.CreateAsync(new RoleButNoCreate());

            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_if_role_and_not_in_role_and_not_administrator_exception_is_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleButNoCreate>();

            Impersonator.AsUser("random-user");

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleButNoCreate()));
        }

        [Fact]
        public async Task When_creating_if_action_allows_create_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<RoleWithCreateAction>();

            Impersonator.AsUser("random-user");
            var entity = await repository.CreateAsync(new RoleWithCreateAction());

            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_if_owned_by_user_and_user_not_logged_in_an_exception_is_thrown()
        {
            var repository = RepositoryFactory.Construct<OwnedByNoCreate>();

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new OwnedByNoCreate()));
        }

        [Fact]
        public async Task When_creating_if_owned_by_user_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<OwnedByNoCreate>();

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "chuck"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new OwnedByNoCreate { OwnedByUserId = "chuck" }));
        }

        [Fact]
        public async Task When_creating_if_owned_by_user_is_current_user_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<OwnedByNoCreate>();

            Impersonator.AsUser("chuck");

            var entity = await repository.CreateAsync(new OwnedByNoCreate { OwnedByUserId = "chuck" });
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_if_owned_by_user_is_different_but_user_is_administrator_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<OwnedByNoCreate>();

            Impersonator.AsAdministrator();

            var entity = await repository.CreateAsync(new OwnedByNoCreate { OwnedByUserId = "chuck" });
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_if_owned_by_user_is_different_but_craete_action_is_allowed_value_is_returned()
        {
            var repository = RepositoryFactory.Construct<OwnedByWithCreateAccess>();

            Impersonator.AsUser("chad");

            var entity = await repository.CreateAsync(new OwnedByWithCreateAccess { OwnedByUserId = "chuck" });
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_creating_both_role_and_owned_by_user_and_role_fails_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();

            Impersonator.AsUser("chad");

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chad" }));
        }

        [Fact]
        public async Task When_creating_both_role_and_owned_by_user_and_owned_by_user_fails_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();

            Impersonator.AsUser("chad").WithRole("other-role");

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chuck" }));
        }

        [Fact]
        public async Task When_creating_both_role_and_owned_by_user_and_both_fail_exception_will_be_thrown()
        {
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();

            Impersonator.AsUser("chad");

            await Assert.ThrowsAsync<SecurityException>(() => repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chuck" }));
        }

        [Fact]
        public async Task When_creating_both_role_and_owned_by_user_and_both_pass_value_will_be_returned()
        {
            var repository = RepositoryFactory.Construct<OwnedByWithCreateAccess>();

            Impersonator.AsUser("chad").WithRole("other-role");

            var entity = await repository.CreateAsync(new OwnedByWithCreateAccess { OwnedByUserId = "chad" });
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task When_loading_if_role_and_user_not_logged_in_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoRead>();
            await repository.CreateAsync(new RoleButNoRead());

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_if_role_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoRead>();
            await repository.CreateAsync(new RoleButNoRead());

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "malicious user"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_if_role_and_in_role_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoRead>();
            var entity = await repository.CreateAsync(new RoleButNoRead());

            HttpContextAccessor.HttpContext = null;
            Impersonator.WithRole("other-role");
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_if_role_and_not_in_role_but_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoRead>();
            var entity = await repository.CreateAsync(new RoleButNoRead());

            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_if_role_and_not_in_role_and_not_administrator_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoRead>();
            await repository.CreateAsync(new RoleButNoRead());

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_if_action_allows_read_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleWithReadAction>();
            var entity = await repository.CreateAsync(new RoleWithReadAction());

            HttpContextAccessor.HttpContext = null;
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_if_owned_by_user_and_user_not_logged_in_an_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoReadParent>();
            await repository.CreateAsync(new OwnedByNoReadParent
            {
                Child = new OwnedByNoRead { OwnedByUserId = "chuck" }
            });

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_if_owned_by_user_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoReadParent>();
            await repository.CreateAsync(new OwnedByNoReadParent
            {
                Child = new OwnedByNoRead { OwnedByUserId = "chuck" }
            });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "chuck"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_if_owned_by_user_is_current_user_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoReadParent>();
            var entity = await repository.CreateAsync(new OwnedByNoReadParent
            {
                Child = new OwnedByNoRead { OwnedByUserId = "chuck" }
            });

            Impersonator.AsUser("chuck");
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_if_owned_by_user_is_different_but_user_is_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoReadParent>();
            var entity = await repository.CreateAsync(new OwnedByNoReadParent
            {
                Child = new OwnedByNoRead { OwnedByUserId = "chuck" }
            });

            Impersonator.AsAdministrator();
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_if_owned_by_user_is_different_but_read_action_is_allowed_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByWithReadAccess>();
            var entity = await repository.CreateAsync(new OwnedByWithReadAccess { OwnedByUserId = "chad" });

            Impersonator.AsUser("someone else");
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_loading_both_role_and_owned_by_user_and_role_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedByParent>();
            await repository.CreateAsync(new RoleAndOwnedByParent
            {
                Child = new RoleAndOwnedBy { OwnedByUserId = "chad" }
            });

            Impersonator.AsUser("chad");

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_both_role_and_owned_by_user_and_owned_by_user_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedByParent>();
            await repository.CreateAsync(new RoleAndOwnedByParent
            {
                Child = new RoleAndOwnedBy { OwnedByUserId = "chad" }
            });

            HttpContextAccessor.HttpContext = null;
            Impersonator.WithRole("this-role");

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_both_role_and_owned_by_user_and_both_fail_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedByParent>();
            await repository.CreateAsync(new RoleAndOwnedByParent
            {
                Child = new RoleAndOwnedBy { OwnedByUserId = "chad" }
            });

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<GenericADOException>(() => repository.GetAsync());
        }

        [Fact]
        public async Task When_loading_both_role_and_owned_by_user_and_both_pass_value_will_be_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedByParent>();
            var entity = await repository.CreateAsync(new RoleAndOwnedByParent
            {
                Child = new RoleAndOwnedBy { OwnedByUserId = "chad" }
            });

            Impersonator.AsUser("chad").WithRole("this-role");
            var entities = await repository.GetAsync();

            Assert.Contains(entity, entities);
        }

        [Fact]
        public async Task When_updating_if_role_and_user_not_logged_in_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoUpdate>();
            var entity = await repository.CreateAsync(new RoleButNoUpdate { Name = "original" });

            HttpContextAccessor.HttpContext = null;
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_if_role_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoUpdate>();
            var entity = await repository.CreateAsync(new RoleButNoUpdate { Name = "original" });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "malicious user"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_if_role_and_in_role_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoUpdate>();
            var entity = await repository.CreateAsync(new RoleButNoUpdate { Name = "original" });

            Impersonator.AsUser("user").WithRole("other-role");
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_if_role_and_not_in_role_but_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoUpdate>();
            var entity = await repository.CreateAsync(new RoleButNoUpdate { Name = "original" });

            Impersonator.AsAdministrator();
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_if_role_and_not_in_role_and_not_administrator_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoUpdate>();
            var entity = await repository.CreateAsync(new RoleButNoUpdate { Name = "original" });

            Impersonator.AsUser("random-user");
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_if_action_allows_create_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleWithUpdate>();
            var entity = await repository.CreateAsync(new RoleWithUpdate { Name = "original" });

            Impersonator.AsUser("random-user");
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_if_owned_by_user_and_user_not_logged_in_an_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoUpdate>();
            var entity = await repository.CreateAsync(new OwnedByNoUpdate { Name = "original" });

            HttpContextAccessor.HttpContext = null;
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_if_owned_by_user_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoUpdate>();
            var entity = await repository.CreateAsync(new OwnedByNoUpdate { Name = "original", OwnedByUserId = "chuck" });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "chuck"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_if_owned_by_user_is_current_user_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoUpdate>();
            var entity = await repository.CreateAsync(new OwnedByNoUpdate { Name = "original", OwnedByUserId = "chuck" });

            Impersonator.AsUser("chuck");
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_if_owned_by_user_is_different_but_user_is_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoUpdate>();
            var entity = await repository.CreateAsync(new OwnedByNoUpdate { Name = "original", OwnedByUserId = "chuck" });

            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_if_owned_by_user_is_different_but_craete_action_is_allowed_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByWithUpdateAccess>();
            var entity = await repository.CreateAsync(new OwnedByWithUpdateAccess { Name = "original", OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad");
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_updating_both_role_and_owned_by_user_and_role_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { Name = "original", OwnedByUserId = "chad" });

            Impersonator.AsUser("chad");
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_both_role_and_owned_by_user_and_owned_by_user_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { Name = "original", OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad").WithRole("this-role");
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_both_role_and_owned_by_user_and_both_fail_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { Name = "original", OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad");
            entity.Name = "updated";

            await Assert.ThrowsAsync<SecurityException>(() => repository.UpdateAsync(entity));
        }

        [Fact]
        public async Task When_updating_both_role_and_owned_by_user_and_both_pass_value_will_be_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { Name = "original", OwnedByUserId = "chad" });

            Impersonator.AsUser("chad").WithRole("this-role");
            entity.Name = "updated";
            await repository.UpdateAsync(entity);

            Assert.Equal(entity.Name, "updated");
        }

        [Fact]
        public async Task When_deleting_if_role_and_user_not_logged_in_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoDelete>();
            var entity = await repository.CreateAsync(new RoleButNoDelete());

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_if_role_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoDelete>();
            var entity = await repository.CreateAsync(new RoleButNoDelete());

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "malicious user"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_if_role_and_in_role_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoDelete>();
            var entity = await repository.CreateAsync(new RoleButNoDelete());

            Impersonator.AsUser("user").WithRole("other-role");
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_if_role_and_not_in_role_but_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoDelete>();
            var entity = await repository.CreateAsync(new RoleButNoDelete());

            Impersonator.AsAdministrator();
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_if_role_and_not_in_role_and_not_administrator_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleButNoDelete>();
            var entity = await repository.CreateAsync(new RoleButNoDelete());

            Impersonator.AsUser("random-user");

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_if_action_allows_delete_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleWithDeleteAction>();
            var entity = await repository.CreateAsync(new RoleWithDeleteAction());

            Impersonator.AsUser("random-user");
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_if_owned_by_user_and_user_not_logged_in_an_exception_is_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoDelete>();
            var entity = await repository.CreateAsync(new OwnedByNoDelete { OwnedByUserId = "chuck" });

            HttpContextAccessor.HttpContext = null;

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_if_owned_by_user_and_user_unauthenticated_an_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoDelete>();
            var entity = await repository.CreateAsync(new OwnedByNoDelete { OwnedByUserId = "chuck" });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimsIdentity.DefaultIssuer, "malicious user"),
                new Claim(ClaimsIdentity.DefaultNameClaimType, "chuck"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "other-role")
            });
            HttpContextAccessor.HttpContext = HttpContextAccessor.HttpContext ?? new DefaultHttpContext();
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_if_owned_by_user_is_current_user_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoDelete>();
            var entity = await repository.CreateAsync(new OwnedByNoDelete { OwnedByUserId = "chuck" });

            Impersonator.AsUser("chuck");
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_if_owned_by_user_is_different_but_user_is_administrator_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByNoDelete>();
            var entity = await repository.CreateAsync(new OwnedByNoDelete { OwnedByUserId = "chuck" });

            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_if_owned_by_user_is_different_but_delete_action_is_allowed_value_is_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<OwnedByWithDeleteAccess>();
            var entity = await repository.CreateAsync(new OwnedByWithDeleteAccess { OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad");
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Fact]
        public async Task When_deleting_both_role_and_owned_by_user_and_role_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chad" });

            Impersonator.AsUser("chad");

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_both_role_and_owned_by_user_and_owned_by_user_fails_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad").WithRole("this-role");

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_both_role_and_owned_by_user_and_both_fail_exception_will_be_thrown()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chuck" });

            Impersonator.AsUser("chad");

            await Assert.ThrowsAsync<SecurityException>(() => repository.DeleteAsync(entity));
        }

        [Fact]
        public async Task When_deleting_both_role_and_owned_by_user_and_both_pass_value_will_be_returned()
        {
            Impersonator.AsAdministrator();
            var repository = RepositoryFactory.Construct<RoleAndOwnedBy>();
            var entity = await repository.CreateAsync(new RoleAndOwnedBy { OwnedByUserId = "chad" });

            Impersonator.AsUser("chad").WithRole("this-role");
            var deleted = await repository.DeleteAsync(entity);

            Assert.NotNull(deleted);
        }

        [Role("other-role")]
        public class RoleButNoCreate : Entity, IAggregateRoot { }

        [Role("other-role")]
        public class RoleButNoRead : Entity, IAggregateRoot { }

        [Role("other-role")]
        public class RoleButNoUpdate : Entity, IAggregateRoot
        {
            public string Name { get; set; }
        }

        [Role("other-role")]
        public class RoleButNoDelete : Entity, IAggregateRoot { }

        [Role(Actions.Create, "other-roles")]
        public class RoleWithCreateAction : Entity, IAggregateRoot { }

        [Role(Actions.Read, "other-roles")]
        public class RoleWithReadAction : Entity, IAggregateRoot { }

        [Role(Actions.Update, "other-roles")]
        public class RoleWithUpdate : Entity, IAggregateRoot
        {
            public string Name { get; set; }
        }

        [Role(Actions.Delete, "other-roles")]
        public class RoleWithDeleteAction : Entity, IAggregateRoot { }

        public class OwnedByNoReadParent : Entity, IAggregateRoot
        {
            public OwnedByNoRead Child { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId))]
        public class OwnedByNoCreate : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId))]
        public class OwnedByNoRead : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId))]
        public class OwnedByNoUpdate : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
            public string Name { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId))]
        public class OwnedByNoDelete : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId), Actions.Create)]
        public class OwnedByWithCreateAccess : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId), Actions.Read)]
        public class OwnedByWithReadAccess : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId), Actions.Update)]
        public class OwnedByWithUpdateAccess : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
            public string Name { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId), Actions.Delete)]
        public class OwnedByWithDeleteAccess : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
        }

        public class RoleAndOwnedByParent : Entity, IAggregateRoot
        {
            public RoleAndOwnedBy Child { get; set; }
        }

        [OwnedByUser(nameof(OwnedByUserId))]
        [Role("this-role")]
        public class RoleAndOwnedBy : Entity, IAggregateRoot
        {
            public string OwnedByUserId { get; set; }
            public string Name { get; set; }
        }
    }
}
