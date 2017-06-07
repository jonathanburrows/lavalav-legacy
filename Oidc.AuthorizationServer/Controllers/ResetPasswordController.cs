using IdentityModel;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    /// <summary>
    ///     Generates reset password tokens, and changes passwords.
    /// </summary>
    [Route("oidc/reset-password")]
    public class ResetPasswordController : ControllerBase
    {
        private PasswordResetter PasswordResetter { get; }
        private UserStore UserStore { get; }
        private PasswordHasher PasswordHasher { get; }

        public ResetPasswordController(PasswordResetter passwordResetter, UserStore userStore, PasswordHasher passwordHasher)
        {
            PasswordResetter = passwordResetter ?? throw new ArgumentNullException(nameof(passwordResetter));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            PasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /// <summary>
        ///     If possible, an email is sent out to the user with a link to reset their password.
        /// </summary>
        /// <param name="username">The username of the user who will be emailed.</param>
        /// <returns>
        ///     If the user exists and has an email, then Ok
        ///     Otherwise, the model errors are returned.
        /// </returns>
        [HttpGet("request/{username}")]
        public async Task<IActionResult> RequestReset(string username)
        {
            var modelState = await ValidateResetRequestAsync(username);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState);
            }
            else
            {
                await PasswordResetter.SendResetPasswordEmailAsync(username);
                return Ok();
            }
        }

        private async Task<ModelStateDictionary> ValidateResetRequestAsync(string username)
        {
            var modelState = new ModelStateDictionary();

            if (string.IsNullOrEmpty(username))
            {
                modelState.AddModelError(nameof(username), "Required");
                return modelState;
            }

            var user = await UserStore.FindByUsernameAsync(username);
            if (user == null)
            {
                modelState.AddModelError(nameof(username), "User not found");
            }
            else if (user.Claims?.Any(u => u.Type == JwtClaimTypes.Email) == false)
            {
                modelState.AddModelError(nameof(username), "No email for this user");
            }

            return modelState;
        }

        /// <summary>
        ///     Will update a user's password, if they have permissions to, and the old password matches.
        /// </summary>
        /// <param name="model">Contains credentials required to reset a users password.</param>
        /// <returns>
        ///     If there are validation errors, a description of what needs to be changed.
        ///     Otherwise, OK
        /// </returns>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordViewModel model)
        {
            // If theres any missing fields, return the errors.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userClaims = (ClaimsIdentity)User.Identity;
            var subject = userClaims.FindFirst(JwtClaimTypes.Subject)?.Value;

            // Return business logic errors.
            var modelState = await ValidateResetAsync(subject, model.OldPassword);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState);
            }

            await PasswordResetter.ResetPassword(subject, model.NewPassword);

            return Ok();
        }

        private async Task<ModelStateDictionary> ValidateResetAsync(string subject, string oldPassword)
        {
            var modelState = new ModelStateDictionary();

            if (!User.Identity.IsAuthenticated)
            {
                modelState.AddModelError(nameof(oldPassword), "You are't logged in");
                return modelState;
            }

            if (subject == null)
            {
                modelState.AddModelError(nameof(oldPassword), "You weren't found");
                return modelState;
            }

            var user = await UserStore.FindBySubjectAsync(subject);
            if (user == null)
            {
                modelState.AddModelError(nameof(oldPassword), "You weren't found");
                return modelState;
            }

            var hashedPassword = PasswordHasher.Hash(oldPassword, user.Salt);
            if (hashedPassword != user.HashedPassword)
            {
                modelState.AddModelError(nameof(oldPassword), "Incorrect password, try again");
            }

            return modelState;
        }
    }
}
