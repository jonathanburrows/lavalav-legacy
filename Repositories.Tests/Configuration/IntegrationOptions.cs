namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Information about each database being used in the integration tests.
    /// </summary>
    public class IntegrationOptions
    {
        public DatabaseOptions MsSql { get; set; }

        public DatabaseOptions Oracle { get; set; }
    }
}