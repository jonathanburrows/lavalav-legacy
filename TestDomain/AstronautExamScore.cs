using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using System.ComponentModel.DataAnnotations;

namespace lvl.TestDomain
{
    [OwnedByUser(nameof(ExamineeUserId))]
    public class AstronautExamScore: Entity, IAggregateRoot
    {
        [Required]
        public string ExamineeUserId { get; set; }
        public bool Passed { get; set; }
        public decimal Score { get; set; }
    }
}
