using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for taking the top amount of records.
    /// </summary>
    internal class TakeQuery<THead, TTail> : ChainedQuery<THead, TTail>
    {
        private int TakeN { get; }

        public TakeQuery(IQuery previous, int takeN) : base(previous) {
            TakeN = takeN;
        }

        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previous = (IQueryable<TTail>)Previous.Apply(querying);
            return previous.Take(TakeN);
        }
    }
}
