using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Ontology.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lvl.TestDomain
{
    [OwnedByUser(nameof(TargetPlanetId), Actions.Read | Actions.Update)]
    [Role("nasa")]
    public class RocketShip: Entity, IAggregateRoot
    {
        public DateTime ExpectedLaunchDate { get; set; }
        public decimal Cost { get; set; }
        public decimal MaximumWeight { get; set; }

        [Required]
        public string InventingUserId { get; set; }
        
        [ForeignKeyId(typeof(Planet))]
        public long TargetPlanetId { get; set; }

        public IEnumerable<Booster> Boosters { get; set; }
    }
}
