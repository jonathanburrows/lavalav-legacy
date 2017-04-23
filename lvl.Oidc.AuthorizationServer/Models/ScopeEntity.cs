using IdentityServer4.Models;
using lvl.Ontology;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(Scope), Schema = "oidc")]
    public class ScopeEntity : IEntity, IAggregateScope<ApiResourceEntity>
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Name of the scope. This is the value a client will use to request the scope.
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
        //     Specifies whether the user can de-select the scope on the consent screen. Defaults
        //     to false.
        public bool Required { get; set; }

        //
        // Summary:
        //     Specifies whether the consent screen will emphasize this scope. Use this setting
        //     for sensitive or important scopes. Defaults to false.
        public bool Emphasize { get; set; }

        //
        // Summary:
        //     Specifies whether this scope is shown in the discovery document. Defaults to
        //     true.
        public bool ShowInDiscoveryDocument { get; set; } = true;

        //
        // Summary:
        //     List of user claims that should be included in the access token.
        public ICollection<UserClaim> UserClaims { get; set; }

        public Scope ToIdentityServer()
        {
            return new IdentityServer4.Models.Scope
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
