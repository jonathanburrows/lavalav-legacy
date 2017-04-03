﻿using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using IdentityServer4.Models;
using lvl.Ontology;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class ScopeEntity : Scope, IEntity
    {
        public int Id { get; set; }
    }

    public class ScopeOverride : IAutoMappingOverride<Scope>
    {
        public void Override(AutoMapping<Scope> mapping)
        {
            mapping.Id(scope => scope.Name);

            mapping.HasMany(scope => scope.UserClaims).Element("Value");
        }
    }
}
