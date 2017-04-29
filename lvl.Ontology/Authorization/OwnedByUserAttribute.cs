using System;

namespace lvl.Ontology.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OwnedByUserAttribute : Attribute
    {
        public string UserIdProperty { get; }
        public Actions ActionsOtherUsersCanPerform { get; }

        public OwnedByUserAttribute(string userIdProperty)
        {
            UserIdProperty = userIdProperty ?? throw new ArgumentNullException(nameof(userIdProperty));
        }

        public OwnedByUserAttribute(Actions actionsOtherUsersCanPerform, string userIdProperty) : this(userIdProperty)
        {
            ActionsOtherUsersCanPerform = actionsOtherUsersCanPerform;
        }
    }
}
