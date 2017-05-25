using lvl.Oidc.AuthorizationServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    /// <summary>
    ///     Performs operations on users.
    /// </summary>
    [Route("oidc/[controller]")]
    public class UserController : ControllerBase
    {
        private UserStore UserStore { get; }

        public UserController(UserStore userStore)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        /// <summary>
        ///     Creates a new user (if valid).
        /// </summary>
        /// <param name="username">The identifier of the user.</param>
        /// <param name="password">The secret authenticator of the user.</param>
        /// <returns>
        ///     If no username given, a model error asking to enter a username.
        ///     
        ///     If the username is already taken, a model error asking to enter a different one.
        ///     
        ///     If no password is given, a model error asking to enter a password.
        ///     
        ///     Otherwise, the created user.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password)
        {
            var modelState = await ValidateUserAsync(username, password);
            if (!modelState.IsValid)
            {
                return BadRequest(modelState);
            }
            else
            {
                var created = await UserStore.AddLocalUserAsync(username, password);
                return Ok(created);
            }
        }

        private async Task<ModelStateDictionary> ValidateUserAsync(string username, string password)
        {
            var modelState = new ModelStateDictionary();

            if (username == null)
            {
                modelState.AddModelError(nameof(username), "Please enter a username");
            }
            else if (await UserStore.FindByUsernameAsync(username) != null)
            {
                modelState.AddModelError(nameof(username), "Already taken, try another");
            }

            if (password == null)
            {
                modelState.AddModelError(nameof(password), "Please enter a password");
            }

            return modelState;
        }
    }
}
