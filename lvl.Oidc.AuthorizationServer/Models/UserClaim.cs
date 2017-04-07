using lvl.Ontology;
using lvl.Ontology.Naming;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Schema("oidc")]
    public class UserClaim : IEntity, IAggregateRoot
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
