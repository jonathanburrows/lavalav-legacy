using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Represents an independent service provider which is allowed.
    /// </summary>
    [Table("IdentityProvider", Schema = "oidc")]
    [HiddenFromApi]
    public class IdentityProviderRestriction : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     The name of an independeant service provider.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
