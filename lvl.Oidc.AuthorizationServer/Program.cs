using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using lvl.Oidc.AuthorizationServer.Seeder;

namespace lvl.Oidc.AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://0.0.0.0:57545")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

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
