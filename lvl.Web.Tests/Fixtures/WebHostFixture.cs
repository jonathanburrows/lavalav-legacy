using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;

namespace lvl.Web.Tests.Fixtures
{
    public class WebHostFixture : IDisposable
    {
        public IServer Server { get; }
        public HttpClient Client { get; }

        public WebHostFixture()
        {
            var webHostBuilder = new WebHostBuilder().UseStartup<TestWebSite.Startup>();

            var testServer = new TestServer(webHostBuilder);
            Server = testServer;

            Client = testServer.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}
