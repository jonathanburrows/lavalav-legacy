using System;
using System.Linq.Expressions;

namespace lvl.Repositories.Querying
{
    /// <summary>
    /// Methods for adding an expression to a chain of queries.
    /// </summary>
    public static class QueryExtensionMethods
    {
        /// <summary>
        /// Creates a query chain with a where clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> Where<THead, TTail>(this IQuery<THead, TTail> query, Expression<Func<TTail, bool>> whereExpression) =>
            new WhereQuery<THead, TTail>((IQuery)query, whereExpression);

        /// <summary>
        /// Creates a query chain with a dynamic where clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> Where<THead, TTail>(this IQuery<THead, TTail> query, string whereExpression) =>
            new DynamicWhereQuery<THead, TTail>((IQuery)query, whereExpression);

        /// <summary>
        /// Creates a query chain with a select clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TResult> Select<THead, TTail, TResult>(this IQuery<THead, TTail> query, Expression<Func<TTail, TResult>> selectExpression) =>
            new SelectQuery<THead, TResult, TTail>((IQuery)query, selectExpression);

        /// <summary>
        /// Creates a query chain with a dynamic select clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, dynamic> Select<THead, TTail>(this IQuery<THead, TTail> query, string selectExpression) =>
            new DynamicSelectQuery<THead, TTail>((IQuery)query, selectExpression);

        /// <summary>
        /// Creates a query chain with a order clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> OrderBy<THead, TTail, TKey>(this IQuery<THead, TTail> query, Expression<Func<TTail, TKey>> keySelector) =>
            new OrderByQuery<THead, TTail, TKey>((IQuery)query, keySelector);

        /// <summary>
        /// Creates a query chain with a dynamic order clause on the end.
        /// </summary>
        /// <remarks>
        /// Does not modify the given query chain.
        /// Used for both ascending and descending queries, for descending, add "fieldName descending".
        /// </remarks>
        public static IQuery<THead, TTail> OrderBy<THead, TTail>(this IQuery<THead, TTail> query, string keySelector) =>
            new DynamicOrderByQuery<THead, TTail>((IQuery)query, keySelector);

        /// <summary>
        /// Creates a query chain with a descending order clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> OrderByDescending<THead, TTail, TKey>(this IQuery<THead, TTail> query, Expression<Func<TTail, TKey>> keySelector) =>
            new OrderByDescendingQuery<THead, TTail, TKey>((IQuery)query, keySelector);

        /// <summary>
        /// Creates a query chain with a skip clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> Skip<THead, TTail>(this IQuery<THead, TTail> query, int skip) =>
            new SkipQuery<THead, TTail>((IQuery)query, skip);

        /// <summary>
        /// Creates a query chain with a take clause on the end.
        /// </summary>
        /// <remarks>Does not modify the given query chain.</remarks>
        public static IQuery<THead, TTail> Take<THead, TTail>(this IQuery<THead, TTail> query, int take) =>
            new TakeQuery<THead, TTail>((IQuery)query, take);
    }
}
