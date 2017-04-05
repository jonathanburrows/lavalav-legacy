using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;
using System.ComponentModel;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Description(nameof(ApiResource))]
    public class ApiResourceEntity : ApiResource, IEntity
    {
        public int Id { get; set; }
    }

    public class ApiResourceOverride : IAutoMappingOverride<ApiResource>
    {
        public void Override(AutoMapping<ApiResource> mapping)
        {
            mapping.Id(apiResource => apiResource.Name);
        }
    }
}
