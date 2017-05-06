using lvl.Ontology;
using System;

namespace lvl.TestDomain
{
    public class Astronaut: IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
