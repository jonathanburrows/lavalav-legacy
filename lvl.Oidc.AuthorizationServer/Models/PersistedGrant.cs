using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class PersistedGrantEntity : PersistedGrant, IEntity
    {
        public int Id { get; set; }
    }

    public class PersistedGrantOverride : IAutoMappingOverride<PersistedGrant>
    {
        public void Override(AutoMapping<PersistedGrant> mapping)
        {
            mapping.Id(persistedGrant => persistedGrant.Key);
        }
    }
}
