using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provide a way to build the middleware pipeline for web requests.
    /// </summary>
    public static class WebMvcBuilderExtensions
    {
        public static IApplicationBuilder UseWeb(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder.UseMvcWithDefaultRoute();

            return applicationBuilder;
        }
    }
}
