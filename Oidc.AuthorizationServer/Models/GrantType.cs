using IdentityServer4.Models;
using lvl.Ontology;
using System.ComponentModel.DataAnnotations.Schema;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Represents authorization flow.
    /// </summary>
    [Table(nameof(GrantType), Schema = "oidc")]
    public class GrantTypeEntity : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     The name of the authorization flow.
        /// </summary>
        public string Name { get; set; }
    }
}
