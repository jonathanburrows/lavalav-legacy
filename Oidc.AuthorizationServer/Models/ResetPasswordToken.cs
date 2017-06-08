using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using lvl.Ontology.Conventions;
using System;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Schema("oidc")]
    [HiddenFromApi]
    public class ResetPasswordToken : Entity, IAggregateRoot
    {
        public Guid Token { get; set; }
        public DateTime CreatedOn { get; set; }

        [ForeignKeyId(typeof(User))]
        public long UserId { get; set; }
    }
}
