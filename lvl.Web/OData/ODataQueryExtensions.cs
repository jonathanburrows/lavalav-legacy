using lvl.Repositories.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace lvl.Web.OData
{
    public static class ODataQueryExtensions
    {
        internal static IQuery<T, T> OrderBy<T>(this IQuery<T, T> query, IQueryCollection queryParameters)
        {
            var orderByValues = new StringValues { };
            if (queryParameters.TryGetValue("$orderby", out orderByValues))
            {
                var orderBys = orderByValues.SelectMany(x => x.Split(','));

                foreach (var orderBy in orderBys.Reverse())
                {
                    var propertyName = orderBy.Split(' ').First();
                    var orderByProperty = typeof(T).GetProperty(propertyName);
                    if (orderByProperty == null)
                    {
                        throw new InvalidOperationException($"Attempting to order by {propertyName} on {typeof(T).Name}, but that property doesnt exist.");
                    }

                    query = query.OrderBy(orderBy);
                }
            }

            return query;
        }

        internal static IQuery<T, T> Skip<T>(this IQuery<T, T> query, IQueryCollection queryParameters)
        {
            var skipValues = new StringValues { };
            if (queryParameters.TryGetValue("$skip", out skipValues))
            {
                var skips = skipValues.FirstOrDefault();
                int skip;
                if (!int.TryParse(skips, out skip))
                {
                    throw new InvalidOperationException($"Attempting to skip the first '{skip}' records, which is not a number.");
                }

                return query.Skip(skip);
            }
            return query;
        }

        internal static IQuery<T, T> Take<T>(this IQuery<T, T> query, IQueryCollection queryParameters)
        {
            var takeValues = new StringValues { };
            if (queryParameters.TryGetValue("$top", out takeValues))
            {
                var takes = takeValues.FirstOrDefault();
                int take;
                if (!int.TryParse(takes, out take))
                {
                    throw new InvalidOperationException($"Attempting to take the first '{take}' records, which is not a number.");
                }

                return query.Take(take);
            }
            return query;
        }

        internal static IQuery Select<T>(this IQuery<T, T> query, IQueryCollection queryParameters)
        {
            var selectValues = new StringValues { };
            if (queryParameters.TryGetValue("$select", out selectValues))
            {
                var selects = selectValues.SelectMany(x => x.Split(','));

                foreach (var select in selects)
                {
                    var selectProperty = typeof(T).GetProperty(select);
                    if (selectProperty == null)
                    {
                        throw new InvalidOperationException($"Attempting to select property {selectProperty} on {typeof(T).Name}, but that property doesnt exist.");
                    }
                }

                var selectExpression = $"new({string.Join(",", selects)})";
                return (IQuery)query.Select(selectExpression);
            }
            return (IQuery)query;
        }

    }
}
