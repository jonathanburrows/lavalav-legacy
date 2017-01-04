using Newtonsoft.Json;

namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Information about each database being used in the integration tests.
    /// </summary>
    public class IntegrationSettings
    {
        [JsonProperty("ms-sql")]
        public DatabaseSettings MsSql { get; set; }

        [JsonProperty("oracle")]
        public DatabaseSettings Oracle { get; set; }
    }
}