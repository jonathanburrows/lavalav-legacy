using lvl.TestDomain;
using lvl.Web.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace lvl.TestWebSite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseWeb();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
        
        public Type[] AssemblyReferences => new[] { typeof(Moon) };
    }
}