using Microsoft.AspNetCore.Http;
using NHibernate;
using System;
using NHibernate.Type;
using lvl.Ontology.Authorization;
using System.Reflection;
using System.Linq;
using System.Security;

namespace lvl.Web.Authorization
{
    /// <summary>
    ///     Prevents a user from performing actions on data they dont have permissions to.
    ///     
    ///     When detected, an exception is thrown, and the transaction handing it should be rolled back.
    /// </summary>
    /// <remarks>
    ///     If the user is an administrator, then they will have access to everything.
    /// </remarks>
    internal class AuthorizationInterceptor : EmptyInterceptor
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AuthorizationInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        ///     When loading a single entity, an exception is thrown if the record belongs to a different user, or another role is required.
        /// </summary>
        /// <returns>
        ///     If the entity has a [Role] attribute:
        ///         and no Read action, and the user is not logged in, then an exception is thrown.
        ///         and no Read action, and the user is unauthenticated, then an exception is thrown.
        ///         and no Read action, and the user isnt in the role, and the user isnt an administrator, then an exception is thrown.
        ///     
        ///     If the entity has a [OwnedByUserAttribute]
        ///         and no Read action, and the user is not logged in, then an exception is thrown.
        ///         and no Read action, and the user is unathenticated, then an exception is thrown.
        ///         and no Read action, and the user id isnt the entity's, and the user isnt an administrator, then an exception is thrown.
        ///         
        ///     Otherwise, the entity is returned.
        /// </returns>
        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Read) || !UserHasPermissionForAction(Actions.Read, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Read);
                throw new SecurityException(message);
            }

            return base.OnLoad(entity, id, state, propertyNames, types);
        }

        /// <summary>
        ///     When creating an entity, an exception is thrown if the record is for a different user, or another role is required.
        /// </summary>
        /// <returns>
        ///     If the entity has a [Role] attribute:
        ///         and no Create action, and the user is not logged in, then an exception is thrown.
        ///         and no Create action, and the user is unauthenticated, then an exception is thrown.
        ///         and no Create action, and the user isnt in the role, and the user isnt an administrator, then an exception is thrown.
        ///         
        ///     If the entity has an [OwnedByUserAttribute]:
        ///         and no Create action, and the user is not logged in, then an exception is thrown.
        ///         and no Create action, and the user is unathenticated, then an exception is thrown.
        ///         and no Create action, and the user id isnt the entity's, and the user isnt an administrator, then an exception is thrown.
        /// </returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Create) || !UserHasPermissionForAction(Actions.Create, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Create);
                throw new SecurityException(message);
            }

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        /// <summary>
        ///     When updating an entity, an exception is thrown if the record is for a different user, or another role is required.
        /// </summary>
        /// <returns>
        ///     If the entity has a [Role] attribute:
        ///         and no Update action, and the user is not logged in, then an exception is thrown.
        ///         and no Update action, and the user is unauthenticated, then an exception is thrown.
        ///         and no Update action, and the user isnt in the role, and the user isnt an administrator, then an exception is thrown.
        ///         
        ///     If the entity has an [OwnedByUserAttribute]:
        ///         and no Update action, and the user is not logged in, then an exception is thrown.
        ///         and no Update action, and the user is unathenticated, then an exception is thrown.
        ///         and no Update action, and the user id isnt the entity's, and the user isnt an administrator, then an exception is thrown.
        ///         and no Update action, and the user id is being updating to a different user, and the userid doesnt match the original, an exception is thrown.
        /// </returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Update) || !UserHasPermissionsToUpdate(entity, currentState, previousState, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Update);
                throw new SecurityException(message);
            }

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }


        /// <summary>
        ///     When deleting an entity, an exception is thrown if the record is for a different user, or another role is required.
        /// </summary>
        /// <returns>
        ///     If the entity has a [Role] attribute:
        ///         and no Delete action, and the user is not logged in, then an exception is thrown.
        ///         and no Delete action, and the user is unauthenticated, then an exception is thrown.
        ///         and no Delete action, and the user isnt in the role, and the user isnt an administrator, then an exception is thrown.
        ///         
        ///     If the entity has an [OwnedByUserAttribute]:
        ///         and no Delete action, and the user is not logged in, then an exception is thrown.
        ///         and no Delete action, and the user is unathenticated, then an exception is thrown.
        ///         and no Delete action, and the user id isnt the entity's, and the user isnt an administrator, then an exception is thrown.
        /// </returns>
        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!UserHasRoleForAction(entity, Actions.Delete) || !UserHasPermissionForAction(Actions.Delete, entity, state, propertyNames))
            {
                var message = GetAuthorizationErrorMessage(Actions.Delete);
                throw new SecurityException(message);
            }

            base.OnDelete(entity, id, state, propertyNames, types);
        }

        private bool UserHasPermissionForAction(Actions action, object entity, object[] state, string[] propertyNames)
        {
            var ownedByUserAttribute = entity.GetType().GetCustomAttribute<OwnedByUserAttribute>();
            if ((ownedByUserAttribute?.ActionsOtherUsersCanPerform & action) != 0)
            {
                return true;
            }

            var user = HttpContextAccessor?.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }
            else if (user.IsInRole("administrator"))
            {
                return true;
            }

            var userId = user.FindFirst("sub")?.Value;

            var ownedByUserIndex = IndexOfOwnedByUserProperty(propertyNames, ownedByUserAttribute.UserIdProperty);
            var currentOwnedById = state[ownedByUserIndex]?.ToString();

            return currentOwnedById == userId;
        }

        private bool UserHasPermissionsToUpdate(object entity, object[] currentState, object[] previousState, string[] propertyNames)
        {
            var ownedByUserAttribute = entity.GetType().GetCustomAttribute<OwnedByUserAttribute>();
            if(ownedByUserAttribute == null)
            {
                return true;
            }

            var userId = HttpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            var ownedByUserIndex = IndexOfOwnedByUserProperty(propertyNames, ownedByUserAttribute.UserIdProperty);
            var previousOwnedById = previousState?[ownedByUserIndex]?.ToString();
            var currentOwnedById = currentState[ownedByUserIndex]?.ToString();
            if (previousOwnedById != currentOwnedById && userId != null && userId == previousOwnedById)
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
            if ((roleAttribute?.ActionsOtherRolesCanPerform & action) != 0)
            {
                return true;
            }

            var user = HttpContextAccessor?.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }
            else if (user.IsInRole("administrator"))
            {
                return true;
            }
            else if (roleAttribute.Roles.Any(allowedRole => user.IsInRole(allowedRole)))
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
            var user = HttpContextAccessor.HttpContext?.User;
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