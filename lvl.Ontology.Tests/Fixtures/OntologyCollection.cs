using lvl.Ontology.Database;
using Xunit;

namespace lvl.Ontology.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class OntologyCollection : 
        ICollectionFixture<InMemoryDomainFixture>,
        ICollectionFixture<DatabaseDetector>
    {
        public const string Name = nameof(OntologyCollection);
    }
}
