using System;
using System.Linq;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    ///     A query for ordering by a given key.
    /// </summary>
    /// <typeparam name="THead"></typeparam>
    /// <typeparam name="TTail"></typeparam>
    /// <typeparam name="TKey">The type of the key being sorted on</typeparam>
    internal class OrderByQuery<THead, TTail, TKey> : ChainedQuery<THead, TTail>
    {
        private Expression<Func<TTail, TKey>> KeySelector { get; }

        public OrderByQuery(IQuery previous, Expression<Func<TTail, TKey>> keySelector) : base(previous)
        {
            KeySelector = keySelector;
        }

        /// <inheritdoc />
        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previousQuery = (IQueryable<TTail>)Previous.Apply(querying);
            return previousQuery.OrderBy(KeySelector);
        }
    }
}
