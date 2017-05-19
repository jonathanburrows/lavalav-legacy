using lvl.Ontology;
using lvl.Ontology.Conventions;
using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Models access to an API resource
    /// </summary>
    [Schema("oidc")]
    public class AllowedScope : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     Tha name of an api resource.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
