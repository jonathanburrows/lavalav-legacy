using IdentityServer4.Models;
using lvl.Ontology;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lvl.Ontology.Conventions;
using lvl.Ontology.Authorization;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     A model for a persisted grant
    /// </summary>
    [Table(nameof(PersistedGrant), Schema = "oidc")]
    [HiddenFromApi]
    public class PersistedGrantEntity : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        [Unique, Required]
        public string Key { get; set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        [Required]
        public string Type { get; set; }

        /// <summary>
        ///     Gets the subject identifier.
        /// </summary>
        /// <value>
        ///     The subject identifier.
        /// </value>
        [Required]
        public string SubjectId { get; set; }

        /// <summary>
        ///     Gets the client identifier.
        /// </summary>
        /// <value>
        ///     The client identifier.
        /// </value>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        ///     Gets or sets the creation time.
        /// </summary>
        /// <value>
        ///     The creation time.
        /// </value>
        public DateTime CreationTime { get; set; }

        /// <summary>
        ///     Gets or sets the expiration.
        /// </summary>
        /// <value>
        ///     The expiration.
        /// </value>
        public DateTime? Expiration { get; set; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        [StringLength(4000)]
        public string Data { get; set; }

        /// <summary>
        ///     Converts POCO to something that IdentityServer can use.
        /// </summary>
        public PersistedGrant ToIdentityPersistedGrant()
        {
            return new PersistedGrant
            {
                ClientId = ClientId,
                CreationTime = CreationTime,
                Data = Data,
                Expiration = Expiration,
                Key = Key,
                SubjectId = SubjectId,
                Type = Type
            };
        }
    }
}
