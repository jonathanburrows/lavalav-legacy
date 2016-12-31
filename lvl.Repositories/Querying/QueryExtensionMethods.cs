using System;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// Methods for adding an expression to a chain of queries
    /// </summary>
    public static class QueryExtensionMethods
    {
        public static IQuery<THead, TTail> Where<THead, TTail>(this IQuery<THead, TTail> query, Expression<Func<TTail, bool>> whereExpression) =>
            new WhereQuery<THead, TTail>((IQuery)query, whereExpression);

        public static IQuery<THead, TTail> Where<THead, TTail>(this IQuery<THead, TTail> query, string whereExpression) =>
            new DynamicWhereQuery<THead, TTail>((IQuery)query, whereExpression);

        public static IQuery<THead, TResult> Select<THead, TTail, TResult>(this IQuery<THead, TTail> query, Expression<Func<TTail, TResult>> selectExpression) =>
            new SelectQuery<THead, TResult, TTail>((IQuery)query, selectExpression);

        public static IQuery<THead, dynamic> Select<THead, TTail>(this IQuery<THead, TTail> query, string selectExpression) =>
            new DynamicSelectQuery<THead, TTail>((IQuery)query, selectExpression);

        public static IQuery<THead, TTail> OrderBy<THead, TTail, TKey>(this IQuery<THead, TTail> query, Expression<Func<TTail, TKey>> keySelector) =>
            new OrderByQuery<THead, TTail, TKey>((IQuery)query, keySelector);

        public static IQuery<THead, TTail> OrderBy<THead, TTail>(this IQuery<THead, TTail> query, string keySelector) =>
            new DynamicOrderByQuery<THead, TTail>((IQuery)query, keySelector);

        public static IQuery<THead, TTail> Skip<THead, TTail>(this IQuery<THead, TTail> query, int skip) =>
            new SkipQuery<THead, TTail>((IQuery)query, skip);

        public static IQuery<THead, TTail> Take<THead, TTail>(this IQuery<THead, TTail> query, int take) =>
            new TakeQuery<THead, TTail>((IQuery)query, take);
    }
}
