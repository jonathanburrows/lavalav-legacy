using lvl.Ontology;

namespace lvl.TestDomain
{
    public class Booster: Entity, IAggregateScope<RocketShip>
    {
        public decimal Thrust { get; set; }
        public decimal BurnTime { get; set; }
    }
}
