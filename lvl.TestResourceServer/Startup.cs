using lvl.DatabaseGenerator;
using lvl.Oidc.AccessTokens.ResourceServer;
using lvl.Oidc.AuthorizationServer;
using lvl.Oidc.AuthorizationServer.Seeder;
using lvl.Ontology;
using lvl.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace lvl.TestResourceServer
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddUserSecrets("2b0f19c1-3546-4658-9715-f6353a59dff8")
                .Build();

        }
        const string url = "http://localhost:56182";

        public void ConfigureServices(IServiceCollection services)
        {
            var oidcAuthorizationServerOptions = new OidcAuthorizationServerOptions(Configuration);
            var webOptions = new WebOptions(Configuration);
            var domainOptions = new DomainOptions(Configuration);
            var resourceServerOptions = new ResourceServerOptions(Configuration);

            services
                .AddDomains(domainOptions)
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb(webOptions)
                .AddOidcAuthorizationServer(oidcAuthorizationServerOptions)
                .AddResourceServer(resourceServerOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseOidcAuthorizationServer()
                .UseResourceServer()
                .UseWeb();

            var databaseGenerator = app.ApplicationServices.GetRequiredService<DatabaseGenerator.DatabaseMigrator>();
            databaseGenerator.Migrate();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            var databaseGenerator = host.Services.GetRequiredService<DatabaseMigrator>();
            databaseGenerator.Migrate();

            var options = host.Services.GetRequiredService<OidcAuthorizationServerOptions>();

            if (options.SeedManditoryData)
            {
                var manditoryDataSeeder = host.Services.GetRequiredService<ManditoryDataSeeder>();
                manditoryDataSeeder.SeedAsync().Wait();
            }

            if (options.SeedTestData)
            {
                var testDataSeeder = host.Services.GetRequiredService<TestDataSeeder>();
                testDataSeeder.SeedAsync().Wait();
            }

            host.Run();
        }
    }
}
