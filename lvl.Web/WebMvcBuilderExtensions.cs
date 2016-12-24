namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provide a way to build the middleware pipeline for web requests.
    /// </summary>
    public static class WebMvcBuilderExtensions
    {
        public static IApplicationBuilder UseWeb(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMvc();

            return applicationBuilder;
        }
    }
}
