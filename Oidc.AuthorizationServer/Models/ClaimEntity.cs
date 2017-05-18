using lvl.Ontology;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Allows a claim to be stored in the database.
    /// </summary>
    [Table(nameof(Claim), Schema = "oidc")]
    public class ClaimEntity : Entity
    {
        /// <summary>
        ///     Gets the claim type of the claim.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Gets the issuer of the claim.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        ///     Gets the value type of the claim.
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        ///     Gets the value of the claim.
        /// </summary>
        public string Value { get; set; }

        public Claim ToSecurityClaim()
        {
            return new Claim(Type, Value, ValueType, Issuer);
        }
    }
}
