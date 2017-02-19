using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// Represents the start of a query, which can be then extended.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Query<T> : ChainedQuery<T, T>
    {
        /// <inheritdoc />
        public override IQueryable<T> Apply(IQueryable<T> querying) => querying;

        /// <inheritdoc />
        public override int Count(IQueryable<T> counting) => counting.Count();
    }
}
