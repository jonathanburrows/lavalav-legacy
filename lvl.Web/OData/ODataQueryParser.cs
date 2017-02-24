using lvl.Repositories.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.OData
{
    /// <summary>
    /// Converts a set of query parameters to a Query.
    /// </summary>
    public class ODataQueryParser
    {
        private ODataConventionTokenizer ConventionTokenizer { get; }
        private ODataConventionParser ConventionParser { get; }

        public ODataQueryParser(ODataConventionTokenizer conventionTokenizer, ODataConventionParser conventionParser)
        {
            if (conventionTokenizer == null)
            {
                throw new ArgumentNullException(nameof(conventionTokenizer));
            }
            if (conventionParser == null) 
            {
                throw new ArgumentNullException(nameof(conventionParser));
            }

            ConventionTokenizer = conventionTokenizer;
            ConventionParser = conventionParser;
        }

        /// <summary>
        /// Converts a set of query parameters to a Query.
        /// </summary>
        /// <typeparam name="T">The type of entites which will be queried.</typeparam>
        /// <param name="parsing">The query which will be converted into a query.</param>
        /// <returns>The query parsed from the query string.</returns>
        public IQuery Parse<T>(IQueryCollection parsing)
        {
            if (parsing == null)
            {
                throw new ArgumentNullException(nameof(parsing));
            }

            var exceptions = GetExceptions<T>(parsing).ToList();
            if (exceptions.Any())
            {
                throw new AggregateException($"{exceptions.Count} invalid odata arguments were given.", exceptions);
            }

            IQuery<T, T> query = new Query<T>();

            var filterValue = GetSingleParameter(parsing, "$filter");
            if (filterValue != null)
            {
                var filterTokens = ConventionTokenizer.Tokenize(filterValue);
                var filterExpression = ConventionParser.Parse(filterTokens);
                var filterStatement = filterExpression.CsString();
                query = query.Where(filterStatement);
            }

            var orderByValues = GetCollectionParameter(parsing, "$orderby");
            foreach (var orderBy in orderByValues.Reverse())
            {
                query = query.OrderBy(orderBy);
            }

            var skipValue = GetSingleParameter(parsing, "$skip");
            if (skipValue != null)
            {
                var skip = int.Parse(skipValue);
                query = query.Skip(skip);
            }

            var topValue = GetSingleParameter(parsing, "$top");
            if (topValue != null)
            {
                var top = int.Parse(topValue);
                query = query.Take(top);
            }

            var selectValues = GetCollectionParameter(parsing, "$select").ToList();
            if (selectValues.Any())
            {
                var selectExpression = $"new({string.Join(",", selectValues)})";
                return (IQuery)query.Select(selectExpression);
            }
            else
            {
                return (IQuery)query;
            }
        }

        private IEnumerable<Exception> GetExceptions<T>(IQueryCollection queryParameters)
        {
            var type = typeof(T);

            var orderBys = GetCollectionParameter(queryParameters, "$orderby");
            foreach (var orderBy in orderBys)
            {
                var propertyName = orderBy.Split(' ').First();
                var orderByProperty = type.GetProperty(propertyName);
                if (orderByProperty == null)
                {
                    yield return new InvalidOperationException($"Attempting to order by {propertyName} on {type.Name}, but that property doesnt exist.");
                }
            }

            var skipValue = GetSingleParameter(queryParameters, "$skip");
            int skip;
            if (skipValue != null && !int.TryParse(skipValue, out skip))
            {
                yield return new InvalidOperationException($"Attempting to skip the first '{skip}' records, which is not a number.");
            }

            var topValue = GetSingleParameter(queryParameters, "$top");
            int top;
            if (topValue != null && !int.TryParse(topValue, out top))
            {
                yield return new InvalidOperationException($"Attempting to take the first '{top}' records, which is not a number.");
            }

            var selectValues = GetCollectionParameter(queryParameters, "$select");
            foreach (var select in selectValues)
            {
                var selectProperty = typeof(T).GetProperty(select);
                if (selectProperty == null)
                {
                    yield return new InvalidOperationException($"Attempting to select property {nameof(selectProperty)} on {typeof(T).Name}, but that property doesnt exist.");
                }
            }
        }

        private static string GetSingleParameter(IQueryCollection queryParameters, string key)
        {
            StringValues values;
            return queryParameters.TryGetValue(key, out values) ? values.First() : default(string);
        }

        private static IEnumerable<string> GetCollectionParameter(IQueryCollection queryParameters, string key)
        {
            StringValues values;
            if (queryParameters.TryGetValue(key, out values))
            {
                return values.SelectMany(v => v.Split(','));
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Converts a set of query parameters to a Query.
        /// </summary>
        /// <param name="parsing">The query which will be converted into a query.</param>
        /// <param name="type">The type of entites which will be queried.</param>
        /// <returns>The query parsed from the query string.</returns>
        public IQuery Parse(IQueryCollection parsing, Type type)
        {
            var genericMethod = GetType().GetMethod(nameof(Parse), new[] { typeof(IQueryCollection) });
            var castedMethod = genericMethod.MakeGenericMethod(type);
            return (IQuery)castedMethod.Invoke(this, new object[] { parsing });
        }
    }
}
