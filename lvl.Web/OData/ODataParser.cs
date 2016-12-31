using lvl.Repositories.Querying;
using Microsoft.AspNetCore.Http;
using System;

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

            return new Query<T>()
                .OrderBy(queryParameters)
                .Skip(queryParameters)
                .Take(queryParameters)
                .Select(queryParameters);
        }

        public IQuery Parse(IQueryCollection parsing, Type type)
        {
            var genericMethod = GetType().GetMethod(nameof(Parse), new[] { typeof(IQueryCollection) });
            var castedMethod = genericMethod.MakeGenericMethod(type);
            return (IQuery)castedMethod.Invoke(this, new[] { parsing });
        }
    }
}
