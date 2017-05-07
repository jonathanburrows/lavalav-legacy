using lvl.Ontology;
using System;
using System.Collections.Generic;

namespace lvl.TestDomain
{
    public class Planet : Entity
    {
        public string Name { get; set; }
        public bool SupportsLife { get; set; }
        public decimal Mass { get; set; }
        public int? AstronomicalUnits { get; set; }
        public DateTime? DiscoveredOn { get; set; }

        public IEnumerable<Moon> Moons { get; set; }
    }
}
