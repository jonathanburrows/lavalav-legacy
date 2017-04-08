using Microsoft.Extensions.Configuration;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    public class GenerationOptions
    {
        public string ConnectionString { get; set; }
        public bool SeedTestData { get; set; }
        public bool SeedManditoryData { get; set; }

        public GenerationOptions() { }

        public GenerationOptions(IConfiguration configuration)
        {
            configuration.GetSection("oidc:authorization-server:generation-options").Bind(this);
        }
    }
}
