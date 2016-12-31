using lvl.TestDomain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

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
    }
}
