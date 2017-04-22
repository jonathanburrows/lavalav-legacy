using lvl.Ontology;
using System.Collections.Generic;
using System.Linq;

namespace lvl.TestResourceServer.Models.Kittens
{
    public class Kitten: IEntity, IAggregateRoot
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Breed { get; set; }
        public Tail Tail { get; set; }
        public IEnumerable<Paw> Paws { get; set; } = Enumerable.Empty<Paw>();
        public int OwnerId { get; set; }
    }
}
