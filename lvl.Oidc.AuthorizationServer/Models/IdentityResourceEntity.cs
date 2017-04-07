using IdentityServer4.Models;
using lvl.Ontology;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(IdentityResource), Schema = "oidc")]
    public class IdentityResourceEntity : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Indicates if this resource is enabled and can be requested. Defaults to true.
        public bool Enabled { get; set; }

        //
        // Summary:
        //     The unique name of the identity resource. This is the value a client will use
        //     for the scope parameter in the authorize request.
        public string Name { get; set; }

        //
        // Summary:
        //     Display name. This value will be used e.g. on the consent screen.
        public string DisplayName { get; set; }

        //
        // Summary:
        //     Description. This value will be used e.g. on the consent screen.
        public string Description { get; set; }

        //
        // Summary:
        //     Specifies whether the user can de-select the scope on the consent screen (if
        //     the consent screen wants to implement such a feature). Defaults to false.
        public bool Required { get; set; }

        //
        // Summary:
        //     Specifies whether the consent screen will emphasize this scope (if the consent
        //     screen wants to implement such a feature). Use this setting for sensitive or
        //     important scopes. Defaults to false.
        public bool Emphasize { get; set; }

        //
        // Summary:
        //     Specifies whether this scope is shown in the discovery document. Defaults to
        //     true.
        public bool ShowInDiscoveryDocument { get; set; }

        //
        // Summary:
        //     List of associated user claims that should be included in the identity token.
        public ICollection<UserClaim> UserClaims { get; set; }

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

        public static IdentityResourceEntity FromIdentityServer(IdentityResource identityResource)
        {
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
