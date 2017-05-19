// ReSharper disable once CheckNamespace In compliance with Microsoft's conventions.
namespace Microsoft.Extensions.DependencyInjection
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