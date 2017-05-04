using Xunit;

namespace lvl.DatabaseGenerator.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class DatabaseGeneratorCollection : 
        ICollectionFixture<InMemoryDatabaseGenerationFixture>,
        ICollectionFixture<ArgumentParser>
    {
        public const string Name = nameof(DatabaseGeneratorCollection);
    }
}
