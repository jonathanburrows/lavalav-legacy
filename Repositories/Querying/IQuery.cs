using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A chain of expressions which can be applied to a query.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Invokes the chain of expressions on a query
        /// </summary>
        /// <param name="querying">The original query which will have expressions applied to it</param>
        /// <returns>The query with all expressions applied</returns>
        IQueryable Apply(IQueryable querying);

        /// <summary>
        /// Counts how many entities fulfill the query criteria.
        /// </summary>
        /// <param name="counting">The query of entities which will be counted.</param>
        /// <returns>The number of entities which match the query criteria.</returns>
        int Count(IQueryable counting);
    }
}
