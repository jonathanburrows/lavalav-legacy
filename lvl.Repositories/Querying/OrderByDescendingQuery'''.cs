using System;
using System.Linq;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for ordering by a given key in descending order.
    /// </summary>
    /// <typeparam name="THead"></typeparam>
    /// <typeparam name="TTail"></typeparam>
    /// <typeparam name="TKey">The type of the key being sorted on.</typeparam>
    internal class OrderByDescendingQuery<THead, TTail, TKey> : ChainedQuery<THead, TTail>
    {
        private Expression<Func<TTail, TKey>> KeySelector { get; }

        public OrderByDescendingQuery(IQuery previous, Expression<Func<TTail, TKey>> keySelector) : base(previous)
        {
            KeySelector = keySelector;
        }

        /// <inheritdoc />
        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previousQuery = (IQueryable<TTail>)Previous.Apply(querying);
            return previousQuery.OrderByDescending(KeySelector);
        }
    }
}
