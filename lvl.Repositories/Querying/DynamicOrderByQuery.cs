using System.Linq;
using System.Linq.Dynamic; 

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for ordering by strings
    /// </summary>
    internal class DynamicOrderByQuery<THead, TTail> : ChainedQuery<THead, TTail>
    {
        private string KeySelector { get; }

        public DynamicOrderByQuery(IQuery previous, string keySelector) : base(previous)
        {
            KeySelector = keySelector;
        }

        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previousQuery = (IQueryable<TTail>)Previous.Apply(querying);
            return previousQuery.OrderBy(KeySelector);
        }
    }
}
