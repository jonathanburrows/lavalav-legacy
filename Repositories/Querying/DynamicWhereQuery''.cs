using System.Linq;
using System.Linq.Dynamic;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for filtering based on a string query.
    /// </summary>
    internal class DynamicWhereQuery<THead, TTail> : ChainedQuery<THead, TTail>
    {
        private string WhereExpression { get; }

        public DynamicWhereQuery(IQuery previous, string whereExpression) : base(previous)
        {
            WhereExpression = whereExpression;
        }

        /// <inheritdoc />
        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previousQuery = (IQueryable<TTail>)Previous.Apply(querying);
            return previousQuery.Where(WhereExpression);
        }

        /// <inheritdoc />
        public override int Count(IQueryable<THead> counting)
        {
            return Apply(counting).Count();
        }
    }
}
