using lvl.Web;
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
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var authorizationOptions = new OidcAuthorizationServerOptions(Configuration);
            var webOptions = new WebSettings(Configuration);

            services
                .AddDomains(authorizationOptions.ConnectionString)
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb(webOptions)
                .AddOidcAuthorizationServer(authorizationOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseOidcAuthorizationServer()
                .UseWeb();
        }
    }
}
