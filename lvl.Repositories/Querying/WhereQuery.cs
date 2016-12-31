using System;
using System.Linq;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query for applying a filter
    /// </summary>
    internal class WhereQuery<THead, TTail> : ChainedQuery<THead, TTail>
    {
        private Expression<Func<TTail, bool>> WhereExpression { get; }

        public WhereQuery(IQuery previous, Expression<Func<TTail, bool>> whereExpression) : base(previous)
        {
            WhereExpression = whereExpression;
        }

        public override IQueryable<TTail> Apply(IQueryable<THead> querying)
        {
            var previous = (IQueryable<TTail>)Previous.Apply(querying);
            return previous.Where(WhereExpression);
        }

        public override int Count(IQueryable<THead> counting)
        {
            return Apply(counting).Count();
        }
    }
}
