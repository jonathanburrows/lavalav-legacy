using lvl.Ontology;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lvl.Repositories
{
    /// <summary>
    /// Manages persistence f0r a set of entities of a certain type.
    /// </summary>
    /// <typeparam name="TEntity">The type of all entities in the repository.</typeparam>
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets all entities in the repository.
        /// </summary>
        /// <returns>All entities in the repository.</returns>
        Task<IEnumerable<TEntity>> GetAsync();

        /// <summary>
        /// Gets an entity with a matching id from the repository.
        /// </summary>
        /// <param name="id">The identifier of the desired entity.</param>
        /// <returns>The matching entity if one exists, null if no matching entity.</returns>
        Task<TEntity> GetAsync(int id);

        /// <summary>
        /// Creates an entity with all properties, and updates the model with any generated values.
        /// </summary>
        /// <param name="creating">The entity to be added.</param>
        /// <returns>The model with all generated values.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="creating"/> cannot be null.</exception>
        Task<TEntity> CreateAsync(TEntity creating);

        /// <summary>
        /// Updates an entity, whos identifier matches the given model, with all the model's fields.
        /// </summary>
        /// <param name="updating">The model whos properties will be applied to the matching entity.</param>
        /// <returns>The model with updated properties which were generated.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="updating"/> cannot be null.</exception>
        /// <exception cref="System.InvalidOperationException">There exists no entity with a matching id.</exception>
        Task<TEntity> UpdateAsync(TEntity updating);

        /// <summary>
        /// Deletes an entity, whos identifier matches the given model.
        /// </summary>
        /// <param name="deleting">The model which was deleted.</param>
        /// <returns>The model which was deleted.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="deleting"/> cannot be null.</exception>
        /// <exception cref="System.InvalidOperationException">There exists no entity with a matching id.</exception>
        Task<TEntity> DeleteAsync(TEntity deleting);
    }
}
