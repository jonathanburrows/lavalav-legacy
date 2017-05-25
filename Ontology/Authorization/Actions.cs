using System;

namespace lvl.Ontology.Authorization
{
    /// <summary>
    ///     Grants a user permissions to perform certain actions.
    /// </summary>
    /// <remarks>
    ///     When multiple actions are allowed, use the bitwise or operator:
    ///         Action.Read | Action.Create | Action.Delete
    /// </remarks>
    [Flags]
    public enum Actions
    {
        Read =   0b0001,
        Create = 0b0010,
        Update = 0b0100,
        Delete = 0b1000
    }
}
