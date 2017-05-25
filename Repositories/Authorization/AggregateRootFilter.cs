using lvl.Ontology;
using System.Linq;

namespace lvl.Repositories.Authorization
{
    /// <summary>
    ///     A filter which will be applied to the root of all queries.
    /// </summary>
    /// <remarks>
    ///     This was placed here to allow the repository to use the filter, while allowing security implementations to override the implementation.
    /// </remarks>
    public class AggregateRootFilter
    {
        /// <summary>
        ///     Return all entities.
        /// </summary>
        public virtual IQueryable<TAggregateRoot> Filter<TAggregateRoot>(IQueryable<TAggregateRoot> filtering) where TAggregateRoot : IAggregateRoot => filtering;
    }
}
