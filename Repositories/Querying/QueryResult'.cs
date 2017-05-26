using System.Collections;
using System.Collections.Generic;

namespace lvl.Repositories.Querying
{
    /// <summary>
    ///     Represents the result of a query.
    /// </summary>
    public class QueryResult<TResult> : IQueryResult<TResult>, IQueryResult
    {
        /// <summary>
        ///     The number of items before paging is applied.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     All items returned from the query.
        /// </summary>
        public IEnumerable<TResult> Items { get; set; }

        IEnumerable IQueryResult.Items
        {
            get => Items;
            set => Items = (IEnumerable<TResult>)value;
        }
    }
}
