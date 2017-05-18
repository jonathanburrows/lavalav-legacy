using lvl.Ontology;
using System.ComponentModel.DataAnnotations.Schema;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Represents an independent service provider which is allowed.
    /// </summary>
    [Table("IdentityProvider", Schema = "oidc")]
    public class IdentityProviderRestriction : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     The name of an independeant service provider.
        /// </summary>
        public string Name { get; set; }
    }
}
