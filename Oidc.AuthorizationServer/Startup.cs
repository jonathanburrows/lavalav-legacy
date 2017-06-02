using lvl.DatabaseGenerator;
using lvl.Oidc.AuthorizationServer.Seeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

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
                .AddUserSecrets("lvl.Oidc.AuthorizationServer")
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDomains(new DomainOptions(Configuration))
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb(new WebOptions(Configuration))
                .AddOidcAuthorizationServer(new OidcAuthorizationServerOptions(Configuration));
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseWeb()
                .UseOidcAuthorizationServer()
                .UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = "http://localhost:5004",
                    RequireHttpsMetadata = false,
                    ApiName = "test-resource-server"
                })
                .UseMvcWithDefaultRoute();
        }
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://0.0.0.0:5004")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var databaseGenerator = host.Services.GetRequiredService<DatabaseMigrator>();
            databaseGenerator.Migrate();

            var seeder = host.Services.GetRequiredService<OidcAuthorizationServerSeeder>();
            seeder.SeedAsync().Wait();

            host.Run();
        }
    }
}
