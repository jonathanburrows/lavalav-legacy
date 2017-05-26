using System.Collections;

namespace lvl.Repositories.Querying
{
    /// <summary>
    ///     Represents the results of a query, with the total number of matching records.
    /// </summary>
    public interface IQueryResult
    {
        int Count { get; set; }
        IEnumerable Items { get; set; }
    }
}
