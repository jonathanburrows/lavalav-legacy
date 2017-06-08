using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;

namespace lvl.TestDomain
{
    [HiddenFromApi]
    public class NasaApplication: Entity, IAggregateRoot { }
}
