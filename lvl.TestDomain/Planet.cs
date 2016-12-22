using lvl.Ontology;
using System.Collections.Generic;

namespace lvl.TestDomain
{
    public class Planet : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool SupportsLife { get; set; }
        public decimal Mass { get; set; }

        public IEnumerable<Moon> Moons { get; set; }
    }
}
