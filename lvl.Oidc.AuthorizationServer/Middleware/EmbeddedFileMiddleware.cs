using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Middleware
{
    public class EmbeddedFileMiddleware
    {
        private RequestDelegate Next { get; }
        private EmbeddedFileProvider EmbeddedFileProvider { get; }

        public EmbeddedFileMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));

            var assembly = GetType().Assembly;
            EmbeddedFileProvider = new EmbeddedFileProvider(assembly);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Response.ContentLength.HasValue)
            {
                var potentialFilePath = $"/wwwroot{httpContext.Request.Path}";
                var file = EmbeddedFileProvider.GetFileInfo(potentialFilePath);
                if (file.Exists)
                {

                    using (var stream = file.CreateReadStream())
                    {
                        var contents = new byte[stream.Length];
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Read(contents, 0, (int)stream.Length);

                        httpContext.Response.ContentLength = stream.Length;
                        httpContext.Response.ContentType = "text";
                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        await httpContext.Response.Body.WriteAsync(contents, 0, (int)stream.Length);

                        return;
                    }
                }
            }

            await Next.Invoke(httpContext);
        }
    }
}
