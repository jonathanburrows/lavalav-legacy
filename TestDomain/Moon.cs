﻿using lvl.Ontology;

namespace lvl.TestDomain
{
    public class Moon : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Radius { get; set; }
        public decimal OrbitalDistance { get; set; }
        public decimal Mass { get; set; }

        public Planet Planet { get; set; }
    }
}