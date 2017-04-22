using lvl.Oidc.AccessTokens.ResourceServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace lvl.TestResourceServer
{
    public class Startup
    {
        //const string url = "http://localhost:5001";

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddResourceServer();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var options = new ResourceServerOptions
            {
                ApiName = "test-resource-server",
                Authority = "http://localhost:65170",
               // Authority = url,
                ApiSecret = "secret",
                RequireHttpsMetadata = false
            };
            
            app.UseResourceServer(options)
                .UseWeb();
        }
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseUrls(url)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
