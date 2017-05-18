using lvl.Ontology;
using lvl.Ontology.Conventions;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     A site which is considered safe.
    /// </summary>
    [Schema("oidc")]
    public class CorsOrigin : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     A url to a site.
        /// </summary>
        public string Name { get; set; }
    }
}
