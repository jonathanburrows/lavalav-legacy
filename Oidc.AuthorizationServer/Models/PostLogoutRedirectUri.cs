using lvl.Ontology;
using lvl.Ontology.Conventions;

namespace lvl.Oidc.AuthorizationServer.Models
{

    /// <summary>
    ///     Determines allowed URIs to redirect to after logout.
    /// </summary>
    [Schema("oidc")]
    public class PostLogoutRedirectUri : Entity, IAggregateScope<ClientEntity>
    {
        /// <summary>
        ///     A url which is allowed to be called after logout.
        /// </summary>
        public string Name { get; set; }
    }
}
