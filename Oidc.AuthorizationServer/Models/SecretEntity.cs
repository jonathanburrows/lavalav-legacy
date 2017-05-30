using IdentityServer4.Models;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static IdentityServer4.IdentityServerConstants;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Models a client secret with identifier and expiration
    /// </summary>
    [Table(nameof(Secret), Schema = "oidc")]
    [HiddenFromApi]
    public class SecretEntity : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the expiration.
        /// </summary>
        /// <value>
        ///     The expiration.
        /// </value>
        public DateTime? Expiration { get; set; }

        /// <summary>
        ///     Gets or sets the type of the client secret.
        /// </summary>
        /// <value>
        ///     The type of the client secret.
        /// </value>
        public string Type { get; set; } = SecretTypes.SharedSecret;

        /// <summary>
        ///     Converts POCO to a secret that can be used by IdentityServer.
        /// </summary>
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
