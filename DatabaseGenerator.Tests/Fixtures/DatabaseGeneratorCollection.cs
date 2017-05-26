using Xunit;

namespace lvl.DatabaseGenerator.Tests.Fixtures
{
    [CollectionDefinition(nameof(DatabaseGeneratorCollection))]
    public class DatabaseGeneratorCollection : 
        ICollectionFixture<InMemoryDatabaseGenerationFixture>,
        ICollectionFixture<ArgumentParser>
    {
    }
}
