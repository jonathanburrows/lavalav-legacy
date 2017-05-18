using lvl.Ontology;
using lvl.Ontology.Conventions;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Specifies allowed URIs to return tokens or authorization codes to.
    /// </summary>
    [Schema("oidc")]
    public class RedirectUri : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     A url which is allowed.
        /// </summary>
        public string Name { get; set; }
    }
}
