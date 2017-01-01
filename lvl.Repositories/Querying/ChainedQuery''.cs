using System;
using System.Linq;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// A query that can be Chained.
    /// </summary>
    public abstract class ChainedQuery<THead, TTail> : IQuery<THead, TTail>, IQuery
    {
        protected IQuery Previous { get; }

        protected ChainedQuery() { }

        protected ChainedQuery(IQuery previous)
        {
            Previous = previous;
        }

        public abstract IQueryable<TTail> Apply(IQueryable<THead> querying);

        IQueryable IQuery.Apply(IQueryable querying)
        {
            var boxed = querying as IQueryable<THead>;
            if (boxed == null)
            {
                throw new ArgumentException($"Expected queryable of type {typeof(THead).Name} but got {querying.ElementType}");
            }
            if (querying == null)
            {
                throw new ArgumentNullException(nameof(querying));
            }

            return Apply(boxed);
        }

        public virtual int Count(IQueryable<THead> counting) => Previous.Count(counting);

        int IQuery.Count(IQueryable counting)
        {
            var boxed = counting as IQueryable<THead>;
            if (boxed == null)
            {
                throw new ArgumentException($"Expected queryable of type {typeof(THead).Name} but got {counting.ElementType}");
            }
            if (counting == null)
            {
                throw new ArgumentNullException(nameof(counting));
            }

            return Count(boxed);
        }
    }
}
