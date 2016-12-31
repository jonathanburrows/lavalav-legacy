using System;
using System.Linq;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query which manipulates the result of the query
    /// </summary>
    /// <typeparam name="THead"></typeparam>
    /// <typeparam name="TTail"></typeparam>
    /// <typeparam name="TSource">The type which the query chain, right before the last expression, produces</typeparam>
    internal class SelectQuery<THead, TTail, TSource> : ChainedQuery<THead, TTail>
    {
        private Expression<Func<TSource, TTail>> SelectExpression { get; }

        public SelectQuery(IQuery previous, Expression<Func<TSource, TTail>> selectExpression) : base(previous)
        {
            SelectExpression = selectExpression;
        }

        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previous = (IQueryable<TSource>)Previous.Apply(querying);
            return previous.Select(SelectExpression);
        }
    }
}
