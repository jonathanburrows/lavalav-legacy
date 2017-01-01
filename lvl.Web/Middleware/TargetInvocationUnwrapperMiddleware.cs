using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace lvl.Web.Middleware
{
    /// <summary>
    /// Since the infastructer uses reflection for generic types, it obfuscates error details, and makes
    /// unit testing for specific errors difficult. This will unwrap reflection errors for debugging purposes.
    /// </summary>
    public class TargetInvocationUnwrapperMiddleware
    {
        private RequestDelegate Next { get; }

        public TargetInvocationUnwrapperMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await Next.Invoke(httpContext);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }
    }
}
