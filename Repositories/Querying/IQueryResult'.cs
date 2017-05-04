using System.Collections.Generic;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// Represents the results of a query, with the total number of matching records.
    /// </summary>
    /// <typeparam name="TResult">The type of query which was executed.</typeparam>
    public interface IQueryResult<TResult>
    {
        int Count { get; set; }
        IEnumerable<TResult> Items { get; set; }
    }
}
