// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Information specific to a database.
    /// </summary>
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// If disabled, any connection string will be ignored.
        /// </summary>
        public bool? Disabled { get; set; }
    }
}