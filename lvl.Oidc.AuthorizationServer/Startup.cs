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
        private OidcAuthorizationServerOptions OidcAuthorizationServerOptions { get; }
        private WebSettings WebOptions { get; }

        public Startup(IHostingEnvironment env)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddUserSecrets("2b0f19c1-3546-4658-9715-f6353a59dff8")
                .Build();
            OidcAuthorizationServerOptions = new OidcAuthorizationServerOptions(configuration);
            WebOptions = new WebSettings(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDomains(OidcAuthorizationServerOptions.ConnectionString)
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb(WebOptions)
                .AddOidcAuthorizationServer(OidcAuthorizationServerOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseOidcAuthorizationServer(OidcAuthorizationServerOptions)
                .UseWeb();
        }
    }
}
