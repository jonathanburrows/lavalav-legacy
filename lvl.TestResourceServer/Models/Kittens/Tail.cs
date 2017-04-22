using lvl.Ontology;

namespace lvl.TestResourceServer.Models.Kittens
{
    public class Tail: IEntity, IAggregateScope<Kitten>
    {
        public int Id { get; set; }
        public decimal Length { get; set; }
        public bool IsFluffly { get; set; }
    }
}
