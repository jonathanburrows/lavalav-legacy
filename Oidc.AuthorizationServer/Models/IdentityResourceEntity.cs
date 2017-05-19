using IdentityServer4.Models;
using lvl.Ontology;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Models a user identity resource.
    /// </summary>
    [Table(nameof(IdentityResource), Schema = "oidc")]
    public class IdentityResourceEntity : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Indicates if this resource is enabled. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     The unique name of the resource.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Display name of the resource.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Description of the resource.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Specifies whether the user can de-select the scope on the consent screen (if the consent screen wants to implement such a feature). Defaults to false.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Specifies whether the consent screen will emphasize this scope (if the consent screen wants to implement such a feature). 
        ///     Use this setting for sensitive or important scopes. Defaults to false.
        /// </summary>
        public bool Emphasize { get; set; }

        /// <summary>
        ///     Specifies whether this scope is shown in the discovery document. Defaults to true.
        /// </summary>
        public bool ShowInDiscoveryDocument { get; set; } = true;

        /// <summary>
        /// List of accociated user claims that should be included when this resource is requested.
        /// </summary>
        public ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();

        /// <summary>
        ///     Converts POCO to something that can be used by Identity Server.
        /// </summary>
        public IdentityResource ToIdentityServer()
        {
            return new IdentityResource
            {
                Description = Description,
                DisplayName = DisplayName,
                Emphasize = Emphasize,
                Enabled = Enabled,
                Name = Name,
                Required = Required,
                ShowInDiscoveryDocument = ShowInDiscoveryDocument,
                UserClaims = UserClaims.Select(uc => uc.Name).ToList()
            };
        }

        /// <summary>
        ///     Converts identity server object to a POCO.
        /// </summary>
        public static IdentityResourceEntity FromIdentityServer(IdentityResource identityResource)
        {
            if(identityResource == null)
            {
                throw new ArgumentNullException(nameof(identityResource));
            }

            return new IdentityResourceEntity
            {
                Description = identityResource.Description,
                DisplayName = identityResource.DisplayName,
                Emphasize = identityResource.Emphasize,
                Enabled = identityResource.Enabled,
                Name = identityResource.Name,
                Required = identityResource.Required,
                ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                UserClaims = identityResource.UserClaims.Select(uc => new UserClaim { Name = uc }).ToList()
            };
        }
    }
}
