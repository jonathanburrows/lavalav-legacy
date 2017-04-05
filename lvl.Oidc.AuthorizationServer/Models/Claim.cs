using lvl.Ontology;
using System.ComponentModel;
using System.Security.Claims;

namespace lvl.Oidc.AuthorizationServer.Models
{
    [Description(nameof(Claim))]
    public class ClaimEntity : IEntity
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Issuer { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }

        public static explicit operator Claim(ClaimEntity arg)
        {
            return new Claim(arg.Type, arg.Value, arg.ValueType, arg.Issuer);
        }

        public static explicit operator ClaimEntity(Claim arg)
        {
            return new ClaimEntity
            {
                Type = arg.Type,
                Issuer = arg.Issuer,
                ValueType = arg.ValueType,
                Value = arg.Value
            };
        }
    }
}
