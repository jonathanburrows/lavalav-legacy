using Xunit;
using Microsoft.Extensions.DependencyInjection;
using lvl.Web.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using lvl.TestDomain;
using System.Threading.Tasks;
using lvl.TestWebSite;
using System.Net.Http;
using System;
using System.Net;
using System.Linq;
using lvl.TestWebSite.Fixtures;
using lvl.Web.Tests.Fixtures;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class CorsTests
    {
        private static CorsSettings Settings { get; } = new CorsSettings
        {
            AllowMethods = new[] { "GET", "POST", "PUT", "DELETE", "NON-STANDARD" },
            AllowOrigins = new[] { "http://localhost/" },
            AllowHeaders = new[] { "ALLOWED-HEADER" },
            ExposedHeaders = new[] { "ALLOWED-EXPOSED-HEADER" }
        };

        private HttpClient Client { get; }

        public CorsTests(WebHostFixture<CorsStartup> webHostFixture)
        {
            Client = webHostFixture.Client;
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public void WhenRequestingOptions_ForAllMethods_CorsResponseIsGiven(string method)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, method);

            // Since Theories dont adhere to the TestCollection rules, the call was made synchronous.
            var response = Client.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("NON-STANDARD")]
        public async Task WhenRequestingOptions_NonStandardAllowedMethodsAreReturned(string nonStandardMethod)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, nonStandardMethod);

            var response = await Client.SendAsync(request);
            var allowedMethods = response.Headers.GetValues(CorsConstants.AccessControlAllowMethods);

            Assert.Contains(nonStandardMethod, allowedMethods);
        }

        [Fact]
        public async Task WhenRequestingOptions_ForUnsupportedMethod_NoAllowedMethodsAreReturned()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, "UNSUPPORTED");

            var response = await Client.SendAsync(request);

            var containsMethods = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowMethods);
            Assert.False(containsMethods);
        }

        [Fact]
        public async Task RequestingOptions_WithNoMethod_Returns404()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhenRequestingOptions_ForNonAllowedOrigin_NoOriginsAreReturned()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, "http://example.com/");
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsOrigin = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowOrigin);
            Assert.False(containsOrigin);
        }

        [Fact]
        public async Task WhenRequestingOptions_ForAllowedOrigin_OriginIsReturned()
        {
            var origin = Client.BaseAddress.ToString();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, origin);
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsOrigin = response.Headers.Any(h => h.Key == CorsConstants.AccessControlAllowOrigin && h.Value.Contains(origin));
            Assert.True(containsOrigin);
        }

        [Fact]
        public async Task WhenRequestingOptions_WithNoOrigin_Returns404()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhenRequestingOptions_WithNonAllowedOriginButAllowedMethods_NoHeadersAreReturned()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, "http://example.com/");
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, "NON-STANDARD");

            var response = await Client.SendAsync(request);

            var containsMethods = response.Headers.Any(header => header.Key == CorsConstants.AccessControlAllowMethods);
            Assert.False(containsMethods);
        }

        [Fact]
        public async Task WhenRequestingOptions_ForNonAllowedHeaders_NoHeadersAreReturned()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);
            request.Headers.Add(CorsConstants.AccessControlRequestHeaders, "NON-ALLOWED-HEADER");

            var response = await Client.SendAsync(request);

            Assert.Empty(response.Headers);
        }

        [Fact]
        public async Task WhenRequestingOptions_ForAllowedHeaders_HeadersAreReturned()
        {
            var header = "ALLOWED-HEADER";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Options
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);
            request.Headers.Add(CorsConstants.AccessControlRequestHeaders, header);

            var response = await Client.SendAsync(request);

            var containsHeader = response.Headers.Any(h => h.Key == CorsConstants.AccessControlAllowHeaders && h.Value.Contains(header));
            Assert.True(containsHeader);
        }

        [Fact]
        public async Task WhenPerformingStandardRequest_ForAllowedExposedHeaders_ExposedHeadersAreReturned()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Client.BaseAddress}api/{nameof(Moon)}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add(CorsConstants.Origin, Client.BaseAddress.ToString());
            request.Headers.Add(CorsConstants.AccessControlRequestMethod, HttpMethod.Get.Method);

            var response = await Client.SendAsync(request);

            var containsExposedHeaders = response.Headers.Any(h => h.Key == CorsConstants.AccessControlExposeHeaders && h.Value.Contains("ALLOWED-EXPOSED-HEADER"));
            Assert.True(containsExposedHeaders);
        }

        public class CorsStartup : Startup
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);
                services.AddSingleton(Settings);
            }
        }
    }
}
