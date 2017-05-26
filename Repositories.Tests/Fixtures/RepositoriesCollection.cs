using Xunit;

namespace lvl.Repositories.Tests.Fixtures
{
    [CollectionDefinition(nameof(RepositoriesCollection))]
    public class RepositoriesCollection : ICollectionFixture<SQLiteRepositoryFixture>
    {
    }
}
