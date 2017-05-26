using lvl.Ontology.Database;
using Xunit;

namespace lvl.Ontology.Tests.Fixtures
{
    [CollectionDefinition(nameof(OntologyCollection))]
    public class OntologyCollection : 
        ICollectionFixture<InMemoryDomainFixture>,
        ICollectionFixture<DatabaseDetector>
    {
    }
}
