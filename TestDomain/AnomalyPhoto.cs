using lvl.Ontology;
using lvl.Ontology.Authorization;
using System.ComponentModel.DataAnnotations;

namespace lvl.TestDomain
{
    [Role(Actions.Read, "nasa")]
    [OwnedByUser(nameof(TakenByUserId), Actions.Read)]
    public class AnomalyPhoto : Entity, IAggregateRoot
    {
        [Required]
        public string Name { get; set; }

        public int Views { get; set; }

        [Required]
        public string TakenByUserId { get; set; }
    }
}
