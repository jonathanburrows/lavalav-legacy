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
        public IQuery Parse<T>(IQueryCollection queryParameters)
        {
            if (queryParameters == null)
            {
                throw new ArgumentNullException(nameof(queryParameters));
            }

            var exceptions = GetExceptions<T>(queryParameters).ToList();
            if (exceptions.Any())
            {
                throw new AggregateException($"{exceptions.Count} invalid odata arguments were given.", exceptions);
            }

            IQuery<T, T> query = new Query<T>();

            var orderByValues = GetCollectionParameter(queryParameters, "$orderby");
            var orderBys = orderByValues.SelectMany(x => x.Split(','));
            foreach (var orderBy in orderBys.Reverse())
            {
                query = query.OrderBy(orderBy);
            }

            var skipValue = GetSingleParameter(queryParameters, "$skip");
            if (skipValue != null)
            {
                var skip = int.Parse(skipValue);
                query = query.Skip(skip);
            }

            var topValue = GetSingleParameter(queryParameters, "$top");
            if (topValue != null)
            {
                var top = int.Parse(topValue);
                query = query.Take(top);
            }

            var selectValues = GetCollectionParameter(queryParameters, "$select");
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
                    yield return new InvalidOperationException($"Attempting to select property {selectProperty} on {typeof(T).Name}, but that property doesnt exist.");
                }
            }
        }

        private string GetSingleParameter(IQueryCollection queryParameters, string key)
        {
            var values = new StringValues { };
            if (queryParameters.TryGetValue(key, out values))
            {
                return values.First();
            }
            else
            {
                return default(string);
            }
        }

        private IEnumerable<string> GetCollectionParameter(IQueryCollection queryParameters, string key)
        {
            var values = new StringValues { };
            if (queryParameters.TryGetValue(key, out values))
            {
                return values.SelectMany(v => v.Split(','));
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public IQuery Parse(IQueryCollection parsing, Type type)
        {
            var genericMethod = GetType().GetMethod(nameof(Parse), new[] { typeof(IQueryCollection) });
            var castedMethod = genericMethod.MakeGenericMethod(type);
            return (IQuery)castedMethod.Invoke(this, new[] { parsing });
        }
    }
}
