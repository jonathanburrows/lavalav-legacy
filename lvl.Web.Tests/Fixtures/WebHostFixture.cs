using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace lvl.Web.Tests.Fixtures
{
    public class WebHostFixture : IDisposable
    {
        private TestServer Server { get; }
        public IServiceProvider Services => Server.Host.Services;
        public HttpClient Client { get; }

        public WebHostFixture()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(InitializeServices)
                .UseStartup<TestWebSite.Startup>();

            Server = new TestServer(webHostBuilder);

            Client = Server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
        protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TestWebSite.Startup).Assembly;

            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));

            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }
    }
}
