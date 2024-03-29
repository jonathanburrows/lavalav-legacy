﻿using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Conventions;

namespace lvl.TestDomain
{
    public class Moon : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public decimal Radius { get; set; }
        public decimal OrbitalDistance { get; set; }
        public decimal Mass { get; set; }

        public Planet Planet { get; set; }

        [ForeignKeyId(typeof(Astronaut))]
        public long? FirstPersonToStepFootId { get; set; }
    }
}
