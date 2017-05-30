using IdentityModel;
using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
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

        public ResetPasswordController(PasswordResetter passwordResetter, UserStore userStore)
        {
            PasswordResetter = passwordResetter ?? throw new ArgumentNullException(nameof(passwordResetter));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
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
            var modelState = await ValidateRequestAsync(username);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState);
            }
            else
            {
                await PasswordResetter.SendResetPasswordEmail(username);
                return Ok();
            }
        }

        private async Task<ModelStateDictionary> ValidateRequestAsync(string username)
        {
            var modelState = new ModelStateDictionary();

            if (string.IsNullOrEmpty(username))
            {
                modelState.AddModelError(nameof(username), "Required");
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
    }
}
