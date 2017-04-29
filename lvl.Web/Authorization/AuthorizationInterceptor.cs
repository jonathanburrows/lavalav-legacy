using Microsoft.AspNetCore.Http;
using NHibernate;
using System;
using NHibernate.Type;
using lvl.Ontology.Authorization;
using System.Reflection;
using System.Linq;

namespace lvl.Web.Authorization
{

    public class AuthorizationInterceptor : EmptyInterceptor
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AuthorizationInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Read) || !UserHasPermissionForAction(Actions.Read, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Read);
                throw new InvalidOperationException(message);
            }

            return base.OnLoad(entity, id, state, propertyNames, types);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Create) || !UserHasPermissionForAction(Actions.Create, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Create);
                throw new InvalidOperationException(message);
            }

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Update) || !UserHasPermissionsToUpdate(entity, currentState, previousState, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Update);
                throw new InvalidOperationException(message);
            }

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Delete) || !UserHasPermissionForAction(Actions.Delete, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Delete);
                throw new InvalidOperationException(message);
            }

            base.OnDelete(entity, id, state, propertyNames, types);
        }

        private bool UserHasPermissionForAction(Actions action, object entity, object[] state, string[] propertyNames)
        {
            var ownedByUserAttribute = entity.GetType().GetCustomAttribute<OwnedByUserAttribute>();
            if (ownedByUserAttribute == null)
            {
                return true;
            }
            if ((ownedByUserAttribute.ActionsOtherUsersCanPerform & action) != 0)
            {
                return true;
            }

            var user = HttpContextAccessor?.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }
            var userId = user.FindFirst("sub");

            var ownedByUserIndex = IndexOfOwnedByUserProperty(propertyNames, ownedByUserAttribute.UserIdProperty);
            var currentOwnedById = state[ownedByUserIndex];

            return currentOwnedById == userId;
        }

        private bool UserHasPermissionsToUpdate(object entity, object[] currentState, object[] previousState, string[] propertyNames)
        {
            var ownedByUserAttribute = entity.GetType().GetCustomAttribute<OwnedByUserAttribute>();
            var userId = HttpContextAccessor.HttpContext.User.FindFirst("sub");

            var ownedByUserIndex = IndexOfOwnedByUserProperty(propertyNames, ownedByUserAttribute.UserIdProperty);
            var previousOwnedById = previousState[ownedByUserIndex];
            var currentOwnedById = currentState[ownedByUserIndex];
            if (previousOwnedById != currentOwnedById && userId == previousOwnedById)
            {
                return true;
            }
            else
            {
                return UserHasPermissionForAction(Actions.Update, entity, currentState, propertyNames);
            }
        }

        private int IndexOfOwnedByUserProperty(string[] propertyNames, string ownedByUserProperty)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] == ownedByUserProperty)
                {
                    return i;
                }
            }

            throw new ArgumentNullException($"No used id property called ${ownedByUserProperty}.");
        }

        private bool UserHasRoleForAction(object entity, Actions action)
        {
            var roleAttribute = entity.GetType().GetCustomAttribute<RoleAttribute>();
            if (roleAttribute == null)
            {
                return true;
            }
            if ((roleAttribute.ActionsOtherRolesCanPerform & action) != 0)
            {
                return true;
            }

            var user = HttpContextAccessor?.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var allowedRoles = roleAttribute.Roles;
            if (allowedRoles.Any(allowedRole => user.IsInRole(allowedRole)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetAuthorizationErrorMessage(Actions action)
        {
            var user = HttpContextAccessor.HttpContext.User;
            if (user == null || !user.Identities.Any())
            {
                return "User is not logged in.";
            }
            else if (!user.Identity.IsAuthenticated)
            {
                return "There was a problem authenticated you";
            }
            else
            {
                return $"You do not have permissions to {action}";
            }
        }
    }
}