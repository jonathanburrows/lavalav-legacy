using System;

namespace lvl.Ontology.Authorization
{
    /// <summary>
    ///     Denotes that a POCO can only have actions performed by users of the given role.
    ///     
    ///     Permissions can be extended to all anonymous users through the Actions property.
    /// </summary>
    /// <remarks>
    ///     Intended to decorate entity POCOs, and will be read by nhibernate interceptors/filters.
    /// 
    ///     Administrators generally ignore these rules, and have full access.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RoleAttribute : Attribute
    {
        /// <summary>
        ///     Users in this role have full permissions to the record. 
        ///     Users not in this role cannot access (unless overriden by ActionsOtherRolesCanPerform).
        /// </summary>
        public string[] Roles { get; }

        /// <summary>
        ///     Will allows all users to perform the given actions, regardless of what roles they are part of.
        /// </summary>
        public Actions ActionsOtherRolesCanPerform { get; }

        public RoleAttribute(params string[] roles)
        {
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        public RoleAttribute(Actions actionsOtherRolesCanPerform, params string[] roles) : this(roles)
        {
            ActionsOtherRolesCanPerform = actionsOtherRolesCanPerform;
        }
    }
}
