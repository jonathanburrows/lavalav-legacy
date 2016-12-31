using System.Collections.Generic;

namespace lvl.Repositories.Querying
{
    public interface IQueryResult<TResult>
    {
        int Count { get; set; }
        IEnumerable<TResult> Items { get; set; }
    }
}
