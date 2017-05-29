using lvl.Oidc.AuthorizationServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    /// <summary>
    ///     Send out emails to users with matching emails with what their username is.
    /// </summary>
    [Route("oidc/recover-username")]
    public class RecoverUsernameController : ControllerBase
    {
        private UsernameRecoverer UsernameRecoverer { get; }

        public RecoverUsernameController(UsernameRecoverer usernameRecoverer)
        {
            UsernameRecoverer = usernameRecoverer ?? throw new ArgumentNullException(nameof(usernameRecoverer));
        }

        /// <summary>
        ///     Sends an email containing 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("{email}")]
        public async Task<IActionResult> RecoverUsername(string email)
        {
            if (!await UsernameRecoverer.AnyUsersHaveEmailAsync(email))
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError(nameof(email), "No user has this email");
                return BadRequest(modelState);
            }
            else
            {
                await UsernameRecoverer.RecoverUsernameAsync(email);
                return Ok();
            }
        }
    }
}
