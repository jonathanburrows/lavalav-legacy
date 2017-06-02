using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.ViewModels
{
    /// <summary>
    ///     A flattened list of roles, so a form can edit them.
    /// </summary>
    /// <remarks>
    ///     This was done because User cant be served from the api, and needed an alternative.
    /// </remarks>
    public class PersonalDetailsViewModel
    {
        /// <summary>
        ///     Value of a user's "email" claim.
        /// </summary>
        [EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        ///     Value of a user's "given_name" claim.
        /// </summary>
        [MaxLength(255)]
        public string FirstName { get; set; }

        /// <summary>
        ///     Value of a user's "family_name" claim.
        /// </summary>
        [MaxLength(255)]
        public string LastName { get; set; }

        /// <summary>
        ///     Value of a user's "phone_number" claim.
        /// </summary>
        [Phone]
        public decimal? PhoneNumber { get; set; }

        /// <summary>
        ///     Value of a user's "job" claim.
        /// </summary>
        [MaxLength(255)]
        public string Job { get; set; }

        /// <summary>
        ///     Value of a user's "location" claim.
        /// </summary>
        [MaxLength(255)]
        public string Location { get; set; }
    }
}
