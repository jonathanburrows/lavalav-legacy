using System.Collections;

namespace lvl.Repositories.Querying
{
    public interface IQueryResult
    {
        int Count { get; set; }
        IEnumerable Items { get; set; }
    }
}
