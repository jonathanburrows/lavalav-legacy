using lvl.Ontology;
using lvl.Ontology.Conventions;

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
        public string Name { get; set; }
    }
}
