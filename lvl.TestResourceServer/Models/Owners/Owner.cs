using lvl.Ontology;

namespace lvl.TestResourceServer.Models.Owners
{
    public class Owner: IEntity, IAggregateRoot
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
