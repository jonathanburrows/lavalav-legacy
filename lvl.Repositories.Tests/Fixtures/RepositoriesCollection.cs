using Xunit;

namespace lvl.Repositories.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class RepositoriesCollection : 
        ICollectionFixture<MsSqlRepositoryFixture>,
        ICollectionFixture<OracleRepositoryFixture>,
        ICollectionFixture<SQLiteRepositoryFixture>
    {
        public const string Name = nameof(RepositoriesCollection);
    }
}
