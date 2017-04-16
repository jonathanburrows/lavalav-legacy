using IdentityServer4.Models;
using lvl.Ontology;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static IdentityServer4.IdentityServerConstants;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Table(nameof(Secret), Schema = "oidc")]
    public class SecretEntity : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Gets or sets the description.
        public string Description { get; set; }

        //
        // Summary:
        //     Gets or sets the value.
        public string Value { get; set; }

        //
        // Summary:
        //     Gets or sets the expiration.
        public DateTime? Expiration { get; set; }

        //
        // Summary:
        //     Gets or sets the type of the client secret.
        public string Type { get; set; } = SecretTypes.SharedSecret;

        public Secret ToIdentityServer()
        {
            return new Secret
            {
                Description = Description,
                Value = Value,
                Expiration = Expiration,
                Type = Type
            };
        }
    }
}
