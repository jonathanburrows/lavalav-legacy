using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Ontology.Conventions;
using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.Models
{

    /// <summary>
    ///     Determines allowed URIs to redirect to after logout.
    /// </summary>
    [Schema("oidc")]
    [HiddenFromApi]
    public class PostLogoutRedirectUri : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     A url which is allowed to be called after logout.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
