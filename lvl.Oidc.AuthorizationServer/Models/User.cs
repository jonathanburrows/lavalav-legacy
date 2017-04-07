using lvl.Ontology;
using lvl.Ontology.Naming;
using System.Collections.Generic;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Schema("oidc")]
    public class User : IEntity, IAggregateRoot
    {
        public int Id { get; set; }

        public string SubjectId { get; set; }

        public string Username { get; set; }

        public string HashedPassword { get; set; }

        public string ProviderName { get; set; }

        public string ProviderSubjectId { get; set; }

        public ICollection<ClaimEntity> Claims { get; set; }
    }
}
