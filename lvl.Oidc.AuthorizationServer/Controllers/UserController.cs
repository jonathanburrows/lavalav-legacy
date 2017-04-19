using lvl.Oidc.AuthorizationServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    [Route("oidc/[controller]")]
    public class UserController : ControllerBase
    {
        private UserStore UserStore { get; }

        public UserController(UserStore userStore)
        {
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password)
        {
            var modelState = ValidateUser(username, password);
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

        private ModelStateDictionary ValidateUser(string username, string password) {
            var modelState = new ModelStateDictionary();

            if(username == null)
            {
                modelState.AddModelError(nameof(username), "Please enter a username");
            }

            var matchingUser = UserStore.FindByUsernameAsync(username);
            if(matchingUser != null)
            {
                modelState.AddModelError(nameof(username), "Username taken, please try another");
            }

            if(password == null)
            {
                modelState.AddModelError(nameof(password), "Please enter a password");
            }

            return modelState;
        }
    }
}
