using Xunit;

namespace lvl.Repositories.Tests.Fixtures
{
    /// <summary>
    /// Was split out from Repository collection to isolate errors when a database server crashes.
    /// </summary>
    [CollectionDefinition(Name)]
    public class RepositoriesOracleCollection : ICollectionFixture<OracleRepositoryFixture>
    {
        public const string Name = nameof(RepositoriesOracleCollection);
    }
}
