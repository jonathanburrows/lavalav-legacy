using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A chain of expressions which can be applied to query.
    /// </summary>
    /// <typeparam name="THead">The type of query which the first expression will be applied to</typeparam>
    /// <typeparam name="TTail">The result of the last query in the expression chain</typeparam>
    public interface IQuery<in THead, out TTail>
    {
        /// <summary>
        /// Invokes the chain of expressions in a query
        /// </summary>
        /// <param name="querying">The query which will have expressions applied</param>
        /// <returns>The query with all expressions applied</returns>
        IQueryable<TTail> Apply(IQueryable<THead> querying);

        /// <summary>
        /// Counts how many entities fulfill the query criteria.
        /// </summary>
        /// <param name="counting">The query of entities which will be counted.</param>
        /// <returns>The number of entities which match the query criteria.</returns>
        int Count(IQueryable<THead> counting);
    }
}
