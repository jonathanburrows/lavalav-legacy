using System.Linq;

namespace lvl.Repositories.Authorization
{
    /// <summary>
    /// A filter which will be applied to the root of all queries
    /// </summary>
    public class AggregateRootFilter
    {
        public virtual IQueryable<TEntity> Filter<TEntity>(IQueryable<TEntity> filtering) => filtering;
    }
}
