using FluentNHibernate.Data;
using lvl.Ontology;
using lvl.Ontology.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace lvl.Oidc.AuthorizationServer.Models
{
    /// <summary>
    ///     Allows a claim to be stored in the database.
    /// </summary>
    [Table(nameof(Claim), Schema = "oidc")]
    [HiddenFromApi]
    public class ClaimEntity : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Gets the claim type of the claim.
        /// </summary>
        [Required]
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
        [Required]
        public string Value { get; set; }

        public Claim ToSecurityClaim()
        {
            return new Claim(Type, Value, ValueType, Issuer);
        }
    }
}
