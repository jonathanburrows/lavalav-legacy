using IdentityServer4.Models;
using lvl.Ontology;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(ApiResource), Schema = "oidc")]
    public class ApiResourceEntity : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Indicates if this API is enabled. Defaults to true.
        public bool Enabled { get; set; }

        //
        // Summary:
        //     The unique name of the API. This value is used for authentication with introspection
        //     and will be added to the audience of the outgoing access token.
        public string Name { get; set; }

        //
        // Summary:
        //     Display name of the API resource.
        public string DisplayName { get; set; }

        //
        // Summary:
        //     Description of the API resource.
        public string Description { get; set; }

        //
        // Summary:
        //     The API secret is used for the introspection endpoint. The API can authenticate
        //     with introspection using the API name and secret.
        public ICollection<SecretEntity> ApiSecrets { get; set; }

        //
        // Summary:
        //     List of accociated user claims that should be included in the access token.
        public ICollection<UserClaim> UserClaims { get; set; }

        //
        // Summary:
        //     An API must have at least one scope. Each scope can have different settings.
        public ICollection<ScopeEntity> Scopes { get; set; }

        public ApiResource ToIdentityServer()
        {
            return new ApiResource
            {
                ApiSecrets = ApiSecrets.Select(a => a.ToIdentityServer()).ToList(),
                Description = Description,
                DisplayName = DisplayName,
                Enabled = Enabled,
                Name = Name,
                Scopes = Scopes.Select(s => s.ToIdentityServer()).ToList(),
                UserClaims = UserClaims.Select(uc => uc.Name).ToList()
            };
        }
    }
}
