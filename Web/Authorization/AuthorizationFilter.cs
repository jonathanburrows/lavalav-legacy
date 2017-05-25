using lvl.Ontology.Authorization;
using lvl.Repositories.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace lvl.Web.Authorization
{
    /// <summary>
    ///     Filters entities based on the users claims and who the entity belongs to.
    /// </summary>
    internal class AuthorizationFilter : AggregateRootFilter
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AuthorizationFilter(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        ///     Removes any entities that belong to a different user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the collection being filtered.</typeparam>
        /// <param name="filtering">The collection of entities to be filtered.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filtering"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The property id does not exist on the type.</exception>
        /// <returns>
        ///     If there is no [OwnedBy] on the entity, then all the records.
        ///     
        ///     If the user is not logged in, then no records.
        ///     
        ///     If the user is not authenticated, then no records.
        ///     
        ///     If the user is an administrator, then all the records.
        ///     
        ///     Otherwise, only the records which have the same user id as the user.
        /// </returns>
        /// <remarks>
        ///     This only filters the aggregate root, not child collections. This is due to bugs which would be caused during saves/deletes.
        /// </remarks>
        public override IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> filtering)
        {
            if(filtering == null)
            {
                throw new ArgumentNullException(nameof(filtering));
            }

            var collectionType = typeof(TEntity);
            var ownedByUserAttribute = collectionType.GetCustomAttribute<OwnedByUserAttribute>();
            if (ownedByUserAttribute != null && collectionType.GetProperty(ownedByUserAttribute.UserIdProperty) == null)
            {
                throw new Exception($"Trying to filter the user on a property {collectionType.Name}.{ownedByUserAttribute.UserIdProperty}, which does not exist.");
            }

            var user = HttpContextAccessor.HttpContext?.User;
            if (ownedByUserAttribute == null)
            {
                return filtering;
            }
            else if ((ownedByUserAttribute.ActionsOtherUsersCanPerform & Actions.Read) != 0)
            {
                return filtering;
            }
            else if(user == null)
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }
            else if (!user.Identity.IsAuthenticated)
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }
            else if (user.IsInRole("administrator"))
            {
                return filtering;
            }
            else
            {
                var userId = user.FindFirst("sub").Value;
                return filtering.Where($"{ownedByUserAttribute.UserIdProperty} = @0", userId);
            }
        }
    }
}
