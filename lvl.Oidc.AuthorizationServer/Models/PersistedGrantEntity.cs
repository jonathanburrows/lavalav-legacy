using IdentityServer4.Models;
using lvl.Ontology;
using System;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class PersistedGrantEntity : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        //
        // Summary:
        //     Gets or sets the key.
        public string Key { get; set; }

        //
        // Summary:
        //     Gets the type.
        public string Type { get; set; }

        //
        // Summary:
        //     Gets the subject identifier.
        public string SubjectId { get; set; }

        //
        // Summary:
        //     Gets the client identifier.
        public string ClientId { get; set; }

        //
        // Summary:
        //     Gets or sets the creation time.
        public DateTime CreationTime { get; set; }

        //
        // Summary:
        //     Gets or sets the expiration.
        public DateTime? Expiration { get; set; }

        //
        // Summary:
        //     Gets or sets the data.
        public string Data { get; set; }

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
