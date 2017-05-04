using Xunit;

namespace lvl.Repositories.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class RepositoriesCollection : ICollectionFixture<SQLiteRepositoryFixture>
    {
        public const string Name = nameof(RepositoriesCollection);
    }
}
