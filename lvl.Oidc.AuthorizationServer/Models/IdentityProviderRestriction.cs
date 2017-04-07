using lvl.Ontology;
using System.ComponentModel.DataAnnotations.Schema;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table("IdentityProvider", Schema = "oidc")]
    public class IdentityProviderRestriction: IEntity, IAggregateScope<ClientEntity>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
