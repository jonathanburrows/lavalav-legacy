using lvl.Ontology;
using lvl.Ontology.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     A user which can be authenticated.
    /// </summary>
    [Schema("oidc")]
    public class User : Entity, IAggregateRoot
    {
        /// <summary>
        ///     The unique name of the user, includes the provider.
        /// </summary>
        [Required, Unique]
        public string SubjectId { get; set; }

        /// <summary>
        ///     The name of the user.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        ///     The password which has been salted and hashed.
        /// </summary>
        public string HashedPassword { get; set; }

        public string Salt { get; set; }

        /// <summary>
        ///     The original creator of the user.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        ///     The unique name given by the provider.
        /// </summary>
        public string ProviderSubjectId { get; set; }

        /// <summary>
        ///     List of descriptions about the identity of the user.
        /// </summary>
        public ICollection<ClaimEntity> Claims { get; set; } = new HashSet<ClaimEntity>();
    }
}
