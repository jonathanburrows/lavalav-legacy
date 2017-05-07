namespace lvl.Ontology
{
    /// <summary>
    ///     Represents an entity which is part of a business process.
    /// </summary>
    /// <remarks>
    ///     An entity should only be in 1 aggregate scope. If it should be in multiple, consider promoting it to an aggregate root.
    /// </remarks>
    /// <typeparam name="TAggregateRoot">
    ///     A reference to the root entity, which represents which business process the aggregate scope belongs to.
    /// </typeparam>
    public interface IAggregateScope<TAggregateRoot> where TAggregateRoot : IAggregateRoot { }
}
