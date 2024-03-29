﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace lvl.Web.Logging
{
    /// <summary>
    ///     Catches any errors in the pipeline and logs them. Also attaches the error message to the response.
    /// </summary>
    internal class ErrorLoggingMiddleware
    {
        private RequestDelegate Next { get; }
        private ILoggerFactory LoggerFactory { get; }

        public ErrorLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        ///     Wraps all actions in the remainder of the pipeline in a try catch, and logs errors when they occur.
        /// </summary>
        /// <param name="httpContext">The request being sent down the pipeline.</param>
        /// <exception cref="ArgumentNullException"><paramref name="httpContext"/> is null.</exception>
        /// <returns>The completion of the request.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            try
            {
                await Next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                // If there's a database issue causing the error, we dont want to obscure it, wrap this in an additional try finally
                try
                {
                    var logger = LoggerFactory.CreateLogger<ErrorLoggingMiddleware>();
                    logger.LogError(0, e, e.Message);
                }
                finally
                {
                    httpContext.Response.Clear();
                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsync(e.Message);
                }
            }
        }
    }
}
