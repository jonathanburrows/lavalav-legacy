using lvl.Ontology;

namespace lvl.TestResourceServer.Models.Kittens
{
    public class Paw: IEntity, IAggregateScope<Kitten>
    {
        public int Id { get; set; }
        public string Color { get; set; }
    }
}
