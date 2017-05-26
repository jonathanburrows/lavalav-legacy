using System.Linq;
using System.Linq.Dynamic;

namespace lvl.Repositories.Querying
{
    /// <summary>
    ///     A query for selecting anonyous objects by query.
    /// </summary>
    internal class DynamicSelectQuery<THead, TSource> : ChainedQuery<THead, dynamic>
    {
        private string SelectExpression { get; }
        // ReSharper disable once StaticMemberInGenericType
        private static object QueryConstructionLock { get; } = new object();

        public DynamicSelectQuery(IQuery previous, string selectExpression) : base(previous)
        {
            SelectExpression = selectExpression;
        }

        /// <inheritdoc />
        public override IQueryable<dynamic> Apply(IQueryable<THead> querying)
        {
            var previousResult = (IQueryable<TSource>)Previous.Apply(querying);

            // The dynamic select statement is not thread safe.
            IQueryable selectResults;
            lock (QueryConstructionLock)
            {
                selectResults = previousResult.Select(SelectExpression);
            }

            return (IQueryable<dynamic>)selectResults;
        }
    }
}
