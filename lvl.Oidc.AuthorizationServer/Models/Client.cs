using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class ClientEntity : Client, IEntity
    {
        public int Id { get; set; }

        [JsonIgnore]
        public ICollection<ClaimEntity> MappedClaims
        {
            get
            {
                return Claims?.Select(c => (ClaimEntity)c).ToList();
            }
            set
            {
                Claims = value.Select(c => (Claim)c).ToList();
            }
        }
    }

    public class ClientOverride : IAutoMappingOverride<Client>
    {
        public void Override(AutoMapping<Client> mapping)
        {
            mapping.Id(client => client.ClientId);

            mapping.HasMany(client => client.IdentityProviderRestrictions).Element("Value");
            mapping.HasMany(client => client.AllowedScopes).Element("Value");
            mapping.HasMany(client => client.AllowedCorsOrigins).Element("Value");
            mapping.HasMany(client => client.AllowedGrantTypes).Element("Value");
            mapping.HasMany(client => client.RedirectUris).Element("Value");
            mapping.HasMany(client => client.PostLogoutRedirectUris).Element("Value");

            mapping.IgnoreProperty(client => client.Claims);
        }
    }
}
