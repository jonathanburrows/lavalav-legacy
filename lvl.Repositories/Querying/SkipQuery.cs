using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for skipping records.
    /// </summary>
    internal class SkipQuery<THead, TTail> : ChainedQuery<THead, TTail>
    {
        private int SkipN { get; }

        public SkipQuery(IQuery previous, int skipN) : base(previous)
        {
            SkipN = skipN;
        }

        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previousQuery = (IQueryable<TTail>)Previous.Apply(querying);
            return previousQuery.Skip(SkipN);
        }
    }
}
