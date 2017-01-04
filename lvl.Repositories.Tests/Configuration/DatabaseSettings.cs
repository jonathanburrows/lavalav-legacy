namespace lvl.Repositories.Tests.Configuration
{
    /// <summary>
    /// Information specific to a database.
    /// </summary>
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// If disabled, any connection string will be ignored.
        /// </summary>
        public bool? Disabled { get; set; }
    }
}
