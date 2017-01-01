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
    public class ODataParser
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

            return new Query<T>()
                .OrderBy(queryParameters)
                .Skip(queryParameters)
                .Take(queryParameters)
                .Select(queryParameters);
        }

        private IEnumerable<Exception> GetExceptions<T>(IQueryCollection queryParameters)
        {
            var type = typeof(T);

            var orderByValues = new StringValues { };
            if (queryParameters.TryGetValue("$orderby", out orderByValues))
            {
                var orderBys = orderByValues.SelectMany(x => x.Split(','));

                foreach (var orderBy in orderBys)
                {
                    var propertyName = orderBy.Split(' ').First();
                    var orderByProperty = type.GetProperty(propertyName);
                    if (orderByProperty == null)
                    {
                        yield return new InvalidOperationException($"Attempting to order by {propertyName} on {type.Name}, but that property doesnt exist.");
                    }
                }
            }

            var skipValues = new StringValues { };
            if (queryParameters.TryGetValue("$skip", out skipValues))
            {
                var skips = skipValues.FirstOrDefault();
                int skip;
                if (!int.TryParse(skips, out skip))
                {
                    yield return new InvalidOperationException($"Attempting to skip the first '{skip}' records, which is not a number.");
                }
            }

            var takeValues = new StringValues { };
            if (queryParameters.TryGetValue("$top", out takeValues))
            {
                var takes = takeValues.FirstOrDefault();
                int take;
                if (!int.TryParse(takes, out take))
                {
                    yield return new InvalidOperationException($"Attempting to take the first '{take}' records, which is not a number.");
                }
            }

            var selectValues = new StringValues { };
            if (queryParameters.TryGetValue("$select", out selectValues))
            {
                var selects = selectValues.SelectMany(x => x.Split(','));

                foreach (var select in selects)
                {
                    var selectProperty = typeof(T).GetProperty(select);
                    if (selectProperty == null)
                    {
                        yield return new InvalidOperationException($"Attempting to select property {selectProperty} on {typeof(T).Name}, but that property doesnt exist.");
                    }
                }
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
