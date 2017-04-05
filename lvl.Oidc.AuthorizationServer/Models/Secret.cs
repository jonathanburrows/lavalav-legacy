using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;
using System.ComponentModel;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Description(nameof(Secret))]
    public class SecretEntity : Secret, IEntity
    {
        public int Id { get; set; }
    }

    public class SecurityOverride : IAutoMappingOverride<Secret>
    {
        public void Override(AutoMapping<Secret> mapping)
        {
            mapping.Id(s => s.Type);
        }
    }
}
