using lvl.TestDomain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace lvl.TestWebSite
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb();
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseWeb();
        }

        public Type[] AssemblyReferences => new[] { typeof(Moon) };

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000")
                .Build();
            host.Run();
        }
    }
}
