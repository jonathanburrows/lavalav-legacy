using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace lvl.Oidc.AuthorizationServer
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json", false)
                .AddJsonFile($"appconfig.{env.EnvironmentName}.json", true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var authorizationOptions = new OidcAuthorizationServerOptions(Configuration);

            services
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseWeb()
                .UseOidcAuthorizationServer();
        }
    }
}
