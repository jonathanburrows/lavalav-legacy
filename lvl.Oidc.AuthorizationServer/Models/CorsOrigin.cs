using lvl.Ontology;
using lvl.Ontology.Naming;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Schema("oidc")]
    public class CorsOrigin: IEntity, IAggregateScope<ClientEntity>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
