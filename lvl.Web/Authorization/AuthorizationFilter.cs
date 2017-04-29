using lvl.Ontology.Authorization;
using lvl.Repositories.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace lvl.Web.Authorization
{
    public class AuthorizationFilter : AggregateRootFilter
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AuthorizationFilter(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public override IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> filtering)
        {
            var user = HttpContextAccessor.HttpContext?.User;
            var ownedByUserAttribute = typeof(TEntity).GetType().GetCustomAttribute<OwnedByUserAttribute>();
            if (ownedByUserAttribute == null)
            {
                return filtering;
            }
            else if ((ownedByUserAttribute.ActionsOtherUsersCanPerform & Actions.Read) != 0)
            {
                return filtering;
            }
            else if(user != null)
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }
            else if (user.Identity.IsAuthenticated)
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }
            else
            {
                return filtering.Where($"{ownedByUserAttribute.UserIdProperty} = '{user.FindFirst("sub")}'");
            }
        }
    }
}
