using IdentityServer4.Models;
using lvl.Ontology;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(ApiResource), Schema = "oidc")]
    public class ApiResourceEntity : Entity, IAggregateRoot
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
        ///     List of accociated user claims that should be included when this resource is requested.
        /// </summary>
        public ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();

        /// <summary>
        ///     The API secret is used for the introspection endpoint. The API can authenticate with introspection using the API name and secret.
        /// </summary>
        public ICollection<SecretEntity> ApiSecrets { get; set; }

        /// <summary>
        ///     An API must have at least one scope. Each scope can have different settings.
        /// </summary>
        public ICollection<ScopeEntity> Scopes { get; set; }

        public ApiResourceEntity() { }

        public ApiResourceEntity(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));

            Scopes = new[]
            {
                new ScopeEntity{ Name = Name, DisplayName = DisplayName }
            };
        }

        /// <summary>
        ///     Convert the POCO to one which can be used by identity server.
        /// </summary>
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
