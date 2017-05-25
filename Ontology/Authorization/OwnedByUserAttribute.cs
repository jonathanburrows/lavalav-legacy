using System;

namespace lvl.Ontology.Authorization
{
    /// <summary>
    ///     Denotes a user owns a record, and no other users should have access to it.
    ///     
    ///     The name of the foreign key property which references the user is stored in UserIdProperty.
    ///     
    ///     Permissions can be extended to all users through the Actions property.
    /// </summary>
    /// <remarks>
    ///     Intended to decorate entity POCOs, and will be read by nhibernate filters.
    /// 
    ///     Administrators generally ignore these rules, and have full access.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class OwnedByUserAttribute : Attribute
    {
        /// <summary>
        ///     The name of the C# property which is a foreign key to the user. The value of this property is the user who owns the record.
        /// </summary>
        public string UserIdProperty { get; }

        /// <summary>
        ///     Will allows all users to perform the given actions.
        /// </summary>
        public Actions ActionsOtherUsersCanPerform { get; }

        public OwnedByUserAttribute(string userIdProperty)
        {
            UserIdProperty = userIdProperty ?? throw new ArgumentNullException(nameof(userIdProperty));
        }

        public OwnedByUserAttribute(string userIdProperty, Actions actionsOtherUsersCanPerform) : this(userIdProperty)
        {
            ActionsOtherUsersCanPerform = actionsOtherUsersCanPerform;
        }
    }
}
