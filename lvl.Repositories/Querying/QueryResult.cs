using System.Collections;
using System.Collections.Generic;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// Represents the result of a query.
    /// </summary>
    public class QueryResult<TResult> : IQueryResult<TResult>, IQueryResult
    {
        public int Count { get; set; }

        public IEnumerable<TResult> Items { get; set; }

        IEnumerable IQueryResult.Items
        {
            get
            {
                return Items;
            }

            set
            {
                Items = (IEnumerable<TResult>)value;
            }
        }
    }
}
