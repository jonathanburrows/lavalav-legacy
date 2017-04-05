using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;
using System.ComponentModel;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Description(nameof(IdentityResource))]
    public class IdentityResourceEntity : IdentityResource, IEntity
    {
        public int Id { get; set; }
    }

    public class IdentityResourceOverride : IAutoMappingOverride<IdentityResource>
    {
        public void Override(AutoMapping<IdentityResource> mapping)
        {
            mapping.Id(identity => identity.Name);
            mapping.HasMany(identity => identity.UserClaims).Element("Value");
        }
    }
}
