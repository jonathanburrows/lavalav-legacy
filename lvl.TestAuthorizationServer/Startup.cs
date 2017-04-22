using lvl.DatabaseGenerator;
using lvl.Oidc.AuthorizationServer;
using lvl.Oidc.AuthorizationServer.Seeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace lvl.TestAuthorizationServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var authorizationOptions = new OidcAuthorizationServerOptions
            {
                SeedManditoryData = true,
                SeedTestData = true
            };

            services
                .AddDomains()
                .AddDatabaseGeneration()
                .AddRepositories()
                .AddWeb()
                .AddOidcAuthorizationServer(authorizationOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage()
                .UseOidcAuthorizationServer()
                .UseWeb();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
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
