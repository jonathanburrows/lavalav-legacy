using IdentityModel;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.ViewModels;
using lvl.Repositories;
using lvl.Repositories.Querying;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Services
{
    /// <summary>
    ///     Provides roles in a model. Can update roles from the flat model.
    /// </summary>
    /// <remarks>
    ///     This was done because User cant be served from the api, and needed an alternative.
    /// </remarks>
    public class PersonalDetailsEditor
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IRepository<User> UserRepository { get; }

        public PersonalDetailsEditor(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Will update a users roles from a flat model.
        /// </summary>
        /// <param name="user">The user whos roles will be updated.</param>
        /// <param name="personalDetails">The model which contains the flattened list of roles.</param>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="personalDetails"/> is null.</exception>
        public async Task UpdateUsersPersonalDetailsAsync(User user, PersonalDetailsViewModel personalDetails)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (personalDetails == null)
            {
                throw new ArgumentNullException(nameof(personalDetails));
            }

            UpdateUserClaim(user, JwtClaimTypes.Email, personalDetails.Email);
            UpdateUserClaim(user, JwtClaimTypes.GivenName, personalDetails.FirstName);
            UpdateUserClaim(user, JwtClaimTypes.FamilyName, personalDetails.LastName);
            UpdateUserClaim(user, JwtClaimTypes.PhoneNumber, personalDetails.PhoneNumber?.ToString());
            UpdateUserClaim(user, "job", personalDetails.Job);
            UpdateUserClaim(user, "location", personalDetails.Location);

            await UserRepository.UpdateAsync(user);
        }

        /// <summary>
        ///     Adds, updates, or removes a claim from a user.
        /// </summary>
        /// <param name="user">The user who owns the claims.</param>
        /// <param name="type">The identifier of the role being modified.</param>
        /// <param name="value">The value of the claim being modified</param>
        /// <remarks>
        ///     If the value is null and the user has no claim of the given type, no action is taken.
        ///     
        ///     If there is no matching claim of the given type, one is added.
        ///     
        ///     If there is no value, then the matching claim is removed from the user.
        ///     
        ///     Otherwise, a claim is added.
        /// </remarks>
        private void UpdateUserClaim(User user, string type, string value)
        {
            var matchingClaim = user.Claims.FirstOrDefault(c => c.Type == type);

            if (matchingClaim == null && string.IsNullOrWhiteSpace(value))
            {
                // ReSharper disable once RedundantJumpStatement want to return, just in case more code is added after last else.
                return;
            }
            else if (matchingClaim == null)
            {
                user.Claims.Add(new ClaimEntity { Type = type, Value = value });
            }
            else if (string.IsNullOrEmpty(value))
            {
                user.Claims = user.Claims.Where(c => c.Type != type).ToList();
            }
            else
            {
                matchingClaim.Value = value;
            }
        }

        /// <summary>
        ///     Converts the users claims into a flat model.
        /// </summary>
        /// <param name="user">The user whos claims are being transformed.</param>
        /// <returns>The claims transformed into a flat model.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is null.</exception>
        public PersonalDetailsViewModel GetPersonalDetails(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = user.Claims ?? new ClaimEntity[0];

            var phoneNumberValue = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.PhoneNumber)?.Value;
            decimal? phoneNumber = decimal.TryParse(phoneNumberValue, out decimal p) ? p : default(decimal?);

            return new PersonalDetailsViewModel
            {
                Email = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value,
                FirstName = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?.Value,
                LastName = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value,
                Job = claims.FirstOrDefault(c => c.Type == "job")?.Value,
                Location = claims.FirstOrDefault(c => c.Type == "location")?.Value,
                PhoneNumber = phoneNumber
            };
        }

        /// <summary>
        ///     Fetches the entity which represents who the user is, based on their claimed subject.
        /// </summary>
        /// <returns>
        ///     If authenticated, the record with the matching subject id.
        /// </returns>
        /// <exception cref="InvalidOperationException">There is no request happening.</exception>
        /// <exception cref="InvalidOperationException">The user is not logged in.</exception>
        /// <exception cref="InvalidOperationException">The user is not authenticated.</exception>
        /// <exception cref="InvalidOperationException">There is no matching user with the given subject.</exception>
        /// <remarks>
        ///     This method was placed in this class becauses:
        ///         1. it's only used by the above methods
        ///         2. having it called by the above methods impacts unit tests
        ///         3. adding a service just for it will add complexity not worth the value.
        ///     </remarks>
        public async Task<User> GetCurrentUserAsync()
        {
            var httpUser = HttpContextAccessor.HttpContext?.User;
            var identity = httpUser?.Identity;
            if (identity?.IsAuthenticated != true)
            {
                throw new InvalidOperationException("User is not logged in");
            }

            var subjectId = httpUser.FindFirst("sub")?.Value;
            if(subjectId == null)
            {
                throw new InvalidOperationException("Current user does not have a subject claim.");
            }

            var userQuery = new Query<User>().Where(u => u.SubjectId == subjectId);
            var users = await UserRepository.GetAsync(userQuery);

            if (!users.Items.Any())
            {
                throw new InvalidOperationException($"No user with id {subjectId}");
            }
            return users.Items.Single();
        }
    }
}
