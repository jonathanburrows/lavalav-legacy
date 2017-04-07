using lvl.Ontology;
using System.Security.Claims;

namespace lvl.Oidc.AuthorizationServer.Models
{
    public class ClaimEntity : IEntity
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Issuer { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }

        public Claim ToSecurityClaim()
        {
            return new Claim(Type, Value, ValueType, Issuer);
        }
    }
}
