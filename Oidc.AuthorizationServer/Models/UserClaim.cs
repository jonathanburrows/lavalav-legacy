using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Ontology.Conventions;
using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Description about who a user is.
    /// </summary>
    [Schema("oidc")]
    [HiddenFromApi]
    public class UserClaim : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Description about the identity of the user.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
