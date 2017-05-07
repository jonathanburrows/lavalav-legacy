namespace lvl.Ontology
{
    /// <summary>
    ///     Represents the main entity in a business model.
    /// </summary>
    /// <remarks>
    ///     When making an object reference, an entity should only reference an aggregate root.
    ///     
    ///     If wanting to reference a non-aggregate root, use a foreign key id instead.
    /// 
    ///     If an entity references a non-aggregate root, and uses an object reference, then an exception will be thrown.
    /// </remarks>
    public interface IAggregateRoot { }
}
