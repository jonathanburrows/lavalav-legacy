namespace lvl.Ontology
{
    /// <summary>
    /// Represents a model which can have CRUD performed on it.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The primary key of the entity.
        /// </summary>
        int Id { get; set; }
    }
}
