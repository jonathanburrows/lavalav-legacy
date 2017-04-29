using System;

namespace lvl.Ontology.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RoleAttribute : Attribute
    {
        public string[] Roles { get; }
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
