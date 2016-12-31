using lvl.Repositories.Querying;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.Extensions.Primitives;
using lvl.Ontology;

namespace lvl.Web.OData
{
    public class ODataParser
    {
        public IQuery Parse<T>(IQueryCollection queryParameters)
        {
            if (queryParameters == null)
            {
                throw new ArgumentNullException(nameof(queryParameters));
            }

            IQuery<T, T> query = new Query<T>();
            var queryType = typeof(T);

            var orderByValues = new StringValues { };
            if (queryParameters.TryGetValue("$orderby", out orderByValues))
            {
                var orderBys = orderByValues.SelectMany(x => x.Split(','));

                foreach (var orderBy in orderBys)
                {
                    var orderByProperty = queryType.GetProperty(orderBy);
                    if (orderByProperty == null)
                    {
                        throw new InvalidOperationException($"Attempting to order by {orderByProperty} on {queryType.Name}, but that property doesnt exist.");
                    }

                    query = query.OrderBy(orderBy);
                }
            }

            var skipValues = new StringValues { };
            if (queryParameters.TryGetValue("$skip", out skipValues))
            {
                var skips = skipValues.FirstOrDefault();
                int skip;
                if (!int.TryParse(skips, out skip))
                {
                    throw new InvalidOperationException($"Attempting to skip the first '{skip}' records, which is not a number.");
                }

                query = query.Skip(skip);
            }

            var takeValues = new StringValues { };
            if (queryParameters.TryGetValue("$top", out takeValues))
            {
                var takes = takeValues.FirstOrDefault();
                int take;
                if (!int.TryParse(takes, out take))
                {
                    throw new InvalidOperationException($"Attempting to take the first '{take}' records, which is not a number.");
                }

                query = query.Take(take);
            }

            var selectValues = new StringValues { };
            if (queryParameters.TryGetValue("$select", out selectValues))
            {
                var selects = selectValues.SelectMany(x => x.Split(','));

                foreach (var select in selects)
                {
                    var selectProperty = queryType.GetProperty(select);
                    if (selectProperty == null)
                    {
                        throw new InvalidOperationException($"Attempting to select property {selectProperty} on {queryType.Name}, but that property doesnt exist.");
                    }
                }

                var selectExpression = $"new({string.Join(",", selects)})";
                return (IQuery)query.Select(selectExpression);
            }

            return (IQuery)query;
        }

        public IQuery Parse(IQueryCollection parsing, Type type)
        {
            var genericMethod = GetType().GetMethod(nameof(Parse), new[] { typeof(IQueryCollection) });
            var castedMethod = genericMethod.MakeGenericMethod(type);
            return (IQuery)castedMethod.Invoke(this, new[] { parsing });
        }
    }
}
