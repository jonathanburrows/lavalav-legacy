using IdentityServer4.Models;
using lvl.Ontology;
using System.ComponentModel.DataAnnotations.Schema;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(GrantType), Schema = "oidc")]
    public class GrantTypeEntity: IEntity, IAggregateScope<ClientEntity>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
