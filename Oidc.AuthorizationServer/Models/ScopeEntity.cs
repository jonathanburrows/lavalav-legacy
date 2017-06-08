using FluentNHibernate.Data;
using IdentityServer4.Models;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Models access to an API resource
    /// </summary>
    [Table(nameof(Scope), Schema = "oidc")]
    [HiddenFromApi]
    public class ScopeEntity : Entity, IAggregateScope<ApiResourceEntity>
    {
        /// <summary>
        ///     Name of the scope. This is the value a client will use to request the scope.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Display name. This value will be used e.g. on the consent screen.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Description. This value will be used e.g. on the consent screen.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Specifies whether the user can de-select the scope on the consent screen. Defaults to false.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Specifies whether the consent screen will emphasize this scope. Use this setting for sensitive or important scopes. Defaults to false
        /// </summary>
        public bool Emphasize { get; set; }

        /// <summary>
        ///     Specifies whether this scope is shown in the discovery document. Defaults to true.
        /// </summary>
        public bool ShowInDiscoveryDocument { get; set; } = true;

        /// <summary>
        ///     List of user claims that should be included in the access token.
        /// </summary>
        public IEnumerable<UserClaim> UserClaims { get; set; }

        /// <summary>
        ///     Converts POCO into something IdentityServer can use.
        /// </summary>
        public Scope ToIdentityServer()
        {
            return new Scope
            {
                Description = Description,
                DisplayName = DisplayName,
                Emphasize = Emphasize,
                Name = Name,
                Required = Required,
                ShowInDiscoveryDocument = ShowInDiscoveryDocument,
                UserClaims = UserClaims.Select(uc => uc.Name).ToList()
            };
        }
    }
}
