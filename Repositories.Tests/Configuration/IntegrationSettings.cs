namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Information about each database being used in the integration tests.
    /// </summary>
    public class IntegrationSettings
    {
        public DatabaseSettings MsSql { get; set; }

        public DatabaseSettings Oracle { get; set; }
    }
}